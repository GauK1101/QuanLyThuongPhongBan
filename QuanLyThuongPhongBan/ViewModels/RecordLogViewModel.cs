using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class RecordLogViewModel : BaseViewModel
    {
        #region 🧩 Commands (Lệnh thao tác từ giao diện)

        // 🔹 Lệnh tải lại dữ liệu (Reload) toàn bộ danh sách
        public ICommand? ReloadDataCommand { get; }

        // 🔹 Lệnh chuyển sang trang trước
        public ICommand? PreviousPageCommand { get; }

        // 🔹 Lệnh chuyển sang trang kế tiếp
        public ICommand? NextPageCommand { get; }

        #endregion



        #region 📋 Danh sách dữ liệu (ObservableCollection)

        // 🔹 Danh sách dữ liệu nhật ký hiển thị trên UI
        // ObservableCollection cho phép UI tự động cập nhật khi danh sách thay đổi
        private ObservableCollection<TbNhatKy>? _list;
        public ObservableCollection<TbNhatKy>? List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged(); // 🔔 Báo cho UI biết dữ liệu đã thay đổi
            }
        }

        // 🔹 Bản ghi nhật ký hiện đang được chọn trong UI (nếu có)
        public TbNhatKy? TbNhatKy { get; set; }

        #endregion



        #region 🔍 Tìm kiếm (Search)

        // 🔹 Chuỗi tìm kiếm do người dùng nhập vào
        // Mỗi khi thay đổi, sẽ tự động gọi hàm RefreshTable() để lọc lại danh sách
        private string? _searchFill = string.Empty;
        public string? SearchFill
        {
            get => _searchFill;
            set
            {
                _searchFill = value;
                OnPropertyChanged();

                // 🔁 Làm mới bảng khi người dùng nhập từ khóa tìm kiếm
                _ = RefreshTable();
            }
        }

        #endregion



        #region 📄 Phân trang (Pagination)

        // 🔹 Số lượng bản ghi hiển thị trên mỗi trang
        private const int PageSize = 25;

        // 🔹 Trang hiện tại người dùng đang xem
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();

                    // 🔔 Cập nhật lại trạng thái của nút "Trước" và "Sau"
                    OnPropertyChanged(nameof(CanGoPrevious));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }

        // 🔹 Tổng số trang (được tính toán sau khi truy vấn tổng số bản ghi)
        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        // 🔹 Cho biết có thể chuyển sang trang trước hay không
        // => false khi CurrentPage <= 1
        public bool CanGoPrevious => CurrentPage > 1;

        // 🔹 Cho biết có thể chuyển sang trang sau hay không
        // => false khi CurrentPage >= TotalPages
        public bool CanGoNext => CurrentPage < TotalPages;

        #endregion


        #region 🧩 Constructor (Khởi tạo ViewModel)

        public RecordLogViewModel()
        {
            // 🔹 Khởi tạo đối tượng nhật ký tạm thời (phục vụ binding)
            TbNhatKy = new TbNhatKy();

            // 🔹 Lệnh chuyển sang trang trước
            PreviousPageCommand = new RelayCommand<object>(
                _ => CanGoPrevious, // Điều kiện cho phép nhấn nút
                _ =>
                {
                    if (CanGoPrevious)
                    {
                        CurrentPage--;
                        _ = RefreshTable(); // Gọi làm mới dữ liệu khi chuyển trang
                    }
                });

            // 🔹 Lệnh chuyển sang trang sau
            NextPageCommand = new RelayCommand<object>(
                _ => CanGoNext, // Điều kiện cho phép nhấn nút
                _ =>
                {
                    if (CanGoNext)
                    {
                        CurrentPage++;
                        _ = RefreshTable(); // Làm mới dữ liệu khi chuyển trang
                    }
                });

            // 🔹 Gọi hàm Load() lần đầu để tải dữ liệu ban đầu
            Load();
        }

        #endregion



        #region ⚙️ Load / Refresh (Tải dữ liệu)

        /// <summary>
        /// Hàm khởi tạo dữ liệu ban đầu khi mở View
        /// </summary>
        private void Load()
        {
            _ = RefreshTable(); // Gọi làm mới dữ liệu
        }

        /// <summary>
        /// Dùng để hủy các tác vụ bất đồng bộ trước đó nếu người dùng thao tác liên tục (ví dụ: nhập tìm kiếm nhanh)
        /// </summary>
        private CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Hủy tác vụ đang chạy để tránh trùng lặp truy vấn DB
        /// </summary>
        private void CancelPreviousTask()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();   // 🛑 Hủy tác vụ cũ
                cancellationTokenSource.Dispose();  // Giải phóng bộ nhớ
            }

            // Tạo mới token cho tác vụ kế tiếp
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Biến SemaphoreSlim để ngăn truy vấn song song (tránh xung đột DBContext)
        /// </summary>
        private static readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Làm mới danh sách dữ liệu (phân trang + tìm kiếm)
        /// </summary>
        private async Task RefreshTable()
        {
            CancelPreviousTask(); // 🧹 Dọn tác vụ cũ

            try
            {
                // ⏱️ Delay nhẹ 300ms để tránh reload liên tục khi người dùng gõ nhanh
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                await _dbLock.WaitAsync(); // 🔒 Khóa để chỉ 1 truy vấn chạy cùng lúc
                try
                {
                    // 🧾 Lấy tổng số bản ghi từ bảng nhật ký
                    var totalCount = await DataProvider.Ins.DB.TbNhatKies.CountAsync();
                    TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

                    // 🧭 Lấy dữ liệu phân trang theo thứ tự thời gian giảm dần
                    var query = await DataProvider.Ins.DB.TbNhatKies
                                            .AsNoTracking()
                                            .OrderByDescending(x => x.ThoiGian)
                                            .Skip((CurrentPage - 1) * PageSize)
                                            .Take(PageSize)
                                            .ToListAsync();

                    // 📋 Gán dữ liệu vào danh sách ObservableCollection
                    if (List == null)
                        List = new ObservableCollection<TbNhatKy>();
                    else
                        List.Clear();

                    foreach (var item in query)
                        List.Add(item);
                }
                finally
                {
                    _dbLock.Release(); // 🔓 Giải phóng khóa truy cập DB
                }
            }
            catch (InvalidOperationException)
            {
                // ⚠️ Bỏ qua lỗi do DbContext đang thực hiện nhiều lệnh cùng lúc
            }
            catch (OperationCanceledException)
            {
                // ⚠️ Bỏ qua lỗi nếu tác vụ bị hủy (do người dùng thao tác nhanh)
            }
            catch (Exception ex)
            {
                // 🟥 Ghi log và thông báo lỗi nếu xảy ra vấn đề khác
                logEx(ex);
            }
        }

        #endregion



        #region 🧾 Logging (Ghi log lỗi)

        /// <summary>
        /// Ghi lỗi ra file Logs/error_yyyyMMdd.txt để tiện theo dõi và khắc phục
        /// </summary>
        private void logEx(Exception ex)
        {
            try
            {
                // 🔹 Tạo thư mục Logs nếu chưa có
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(logDir);

                // 🔹 Đặt tên file log theo ngày
                string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.txt");

                // 🔹 Nội dung log chi tiết gồm thời gian, thông báo và stack trace
                string logMessage =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    "----------------------------------------\n";

                // 🔹 Ghi nối thêm vào file log
                File.AppendAllText(logFile, logMessage);

                // 🔹 Hiển thị thông báo nhẹ cho người dùng
                MessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file log để biết thêm chi tiết.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // ⚠️ Tránh crash nếu việc ghi log gặp sự cố (ví dụ: bị khóa file)
            }
        }

        #endregion
    }
}
