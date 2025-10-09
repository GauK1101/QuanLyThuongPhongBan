using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using static QuanLyThuongPhongBan.CLass.SearchFilter;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class ProjectRewardViewModel : BaseViewModel
    {
        public ICommand? ShowAddOrEditRowCommand { get; }
        public ICommand? AddOrEditRowCommand { get; }
        public ICommand? DeleteRowCommand { get; }
        public ICommand? ReloadDataCommand { get; }
        public ICommand? CalculatorDataCommand { get; }
        public ICommand? PreviousPageCommand { get; }
        public ICommand? NextPageCommand { get; }

        private ObservableCollection<TbThuongDuAn>? _list;
        public ObservableCollection<TbThuongDuAn>? List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        public TbThuongDuAn? TbThuongDuAn { get; set; }
        public TbThuongDaiDoanDuAn? TbThuongDaiDoanDuAn { get; set; }
        AddEditProjectRewardWindow? addEditProjectRewardWindow;

        private string? _searchFill = string.Empty;
        public string? SearchFill
        {
            get => _searchFill;
            set
            {
                _searchFill = value;
                OnPropertyChanged();
                _ = RefreshTable();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ProjectRewardViewModel()
        {
            TbThuongDaiDoanDuAn = new TbThuongDaiDoanDuAn();
            TbThuongDuAn = new TbThuongDuAn();

            Load();

            ShowAddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, ShowAddOrEditRow);
            AddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, _ => AddOrEditRow());
            DeleteRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, DeleteRow);
            CalculatorDataCommand = new RelayCommand<TbThuongDuAn>(_ => true, CalculatorData);
            ReloadDataCommand = new RelayCommand<object>(_ => true, _ => Load());
        }

        private void Load()
        {
            _ = RefreshTable();
        }

        private void ShowAddOrEditRow(TbThuongDuAn model)
        {
            if (TbThuongDuAn == null) return;

            if (model == null)
            {

                model = new TbThuongDuAn();

                for (int i = 1; i <= 5; i++)
                {
                    TbThuongDaiDoanDuAn detail = new TbThuongDaiDoanDuAn
                    {
                        IdPhongBan = i,
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i),
                    };
                    model.TbThuongDaiDoanDuAns.Add(detail);
                }

                model.NamThuong = DateTime.Now.Date.Year.ToString();
            }

            TbThuongDuAn = model;

            addEditProjectRewardWindow = new AddEditProjectRewardWindow();
            addEditProjectRewardWindow.ShowDialog();
        }

        private void CalculatorData(TbThuongDuAn model)
        {
            if (TbThuongDuAn == null) return;

            foreach (var detail in TbThuongDuAn.TbThuongDaiDoanDuAns)
            {
                detail.GiaTriTongGoi = (TbThuongDuAn.QuyetToan * detail.TiLeTongGoi) / 100;
                detail.GiaTriDieuChinhDot1 = (TbThuongDuAn.GiaTriHopDong * detail.TiLeDieuChinhDot1) / 100;
                detail.GiaTriDieuChinhDot2 = (TbThuongDuAn.GiaTriHopDong * detail.TiLeDieuChinhDot2) / 100;
                detail.ThuHoiCongNo = detail.GiaTriTongGoi - detail.GiaTriDieuChinhDot1 - detail.GiaTriDieuChinhDot2;
                detail.NghiemThu = detail.GiaTriDieuChinhDot1 + detail.GiaTriDieuChinhDot2 + detail.ThuHoiCongNo;

                TbThuongDuAn.TongTiLeThuongDuAn = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeTongGoi);
                TbThuongDuAn.TongGiaTriThuongDuAn = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriTongGoi);
                TbThuongDuAn.TongTiLeDieuChinhDot1 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeDieuChinhDot1);
                TbThuongDuAn.TongGiaTriDieuChinhDot1 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriDieuChinhDot1);
                TbThuongDuAn.TongTiLeDieuChinhDot2 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeDieuChinhDot2);
                TbThuongDuAn.TongGiaTriDieuChinhDot2 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriDieuChinhDot2);
                TbThuongDuAn.TongThuHoiCongNo = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.ThuHoiCongNo);
                TbThuongDuAn.TongNghiemThu = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.NghiemThu);
            }
        }

        private void AddOrEditRow()
        {
            if (TbThuongDuAn == null) return;

            try
            {
                foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;

                if (TbThuongDuAn.Id == 0)
                {
                    DataProvider.Ins.DB.TbThuongDuAns.Add(TbThuongDuAn);

                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                        ThoiGian = DateTime.Now,
                        HanhDong = "Thêm thưởng dự án",
                        MoTa = $"Thêm mới thưởng dự án - Năm: {TbThuongDuAn.NamThuong}"
                    });

                    DataProvider.Ins.DB.SaveChanges();
                    if (MessageBox.Show("Tạo mới thành công!\nBạn muốn về màn hình chính không ?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                             .OfType<Window>()
                                             .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }
                }
                else
                {
                    DataProvider.Ins.DB.TbThuongDuAns.Update(TbThuongDuAn);
                    DataProvider.Ins.DB.SaveChanges();

                    if (MessageBox.Show("Sửa thành công!\nBạn muốn về màn hình chính không ?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                             .OfType<Window>()
                                             .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }
                }
            }
            catch (DbUpdateException dbUpdateEx)
            {
                string errorMessage;

                // Kiểm tra lỗi cụ thể
                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 2627: // Trùng khóa chính
                            errorMessage = "Trùng khóa chính. Dữ liệu đã tồn tại.";
                            break;
                        case 547: // Vi phạm ràng buộc
                            errorMessage = "Vi phạm ràng buộc. Xin kiểm tra dữ liệu.";
                            break;
                        default:
                            errorMessage = "Lỗi không xác định: " + sqlEx.Message;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi không xác định trong quá trình cập nhật dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Lỗi thực hiện thao tác", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logEx(ex);
            }
        }

        private void DeleteRow(TbThuongDuAn model)
        {
            try
            {
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?", "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;

                DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                {
                    IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                    ThoiGian = DateTime.Now,
                    HanhDong = "Xoá thưởng dự án",
                    MoTa = $"Xoá thưởng dự án - Năm: {model.NamThuong}"
                });

                DataProvider.Ins.DB.TbThuongDuAns.Remove(model);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Xoá thưởng dự án thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                _ = RefreshTable();
            }
            catch (DbUpdateException dbUpdateEx)
            {
                string errorMessage;

                // Kiểm tra lỗi cụ thể
                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 547: // Vi phạm ràng buộc ngoại
                            errorMessage = "Không thể xóa vì có ràng buộc ngoại. Xin kiểm tra dữ liệu liên quan.";
                            break;
                        default:
                            errorMessage = "Lỗi không xác định: " + sqlEx.Message;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi không xác định trong quá trình xóa dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                logEx(ex);
            }
        }

        private CancellationTokenSource? cancellationTokenSource;
        private void CancelPreviousTask()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();
        }

        private static readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1, 1);

        private async Task RefreshTable()
        {
            CancelPreviousTask();

            try
            {
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                await _dbLock.WaitAsync(); // 🔒 Chặn truy vấn song song
                try
                {
                    // Truy vấn cơ sở dữ liệu với phân trang và tìm kiếm trực tiếp
                    var query = DataProvider.Ins.DB.TbThuongDuAns
                                .AsNoTracking()
                                .AsQueryable();

                    // Áp dụng tìm kiếm trực tiếp trên cơ sở dữ liệu

                    var queryDaiDoan = await DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();

                    var queryDuAnChiTiet = await DataProvider.Ins.DB.TbThuongDuAnChiTiets
                        .AsNoTracking()
                        .ToListAsync();

                    if (List == null)
                        List = new ObservableCollection<TbThuongDuAn>();
                    List.Clear();

                    foreach (var item in query)
                    {
                        var tbThuongDaiDoanDuAn = queryDaiDoan
                            .Where(d => d.IdThuongDuAn == item.Id)
                            .ToList();

                        foreach (var t in tbThuongDaiDoanDuAn)
                            item.TbThuongDaiDoanDuAns.Add(t);

                        var tbThuongDuAnChiTiet = queryDuAnChiTiet
                            .Where(d => d.IdThuongDuAn == item.Id)
                            .ToList();

                        foreach (var t in tbThuongDuAnChiTiet)
                            item.TbThuongDuAnChiTiets.Add(t);

                        List.Add(item);
                    }
                }
                finally
                {
                    _dbLock.Release(); // 🔓 Giải phóng lock
                }
            }
            catch (InvalidOperationException)
            {
                // Bỏ qua nếu nếu liên quang đến thông báo DbContext chạy nhiều lệnh
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua nếu tác vụ bị hủy
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void logEx(Exception ex)
        {
            // Đường dẫn thư mục Logs (cùng nơi chạy app)
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDir); // Tự tạo nếu chưa có

            // Tên file log (theo ngày)
            string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.txt");

            // Nội dung log
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}\n{ex.StackTrace}\n----------------------------------------\n";

            // Ghi log
            File.AppendAllText(logFile, logMessage);

            // Hiện thông báo cho người dùng
            MessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file log để biết thêm chi tiết.",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
