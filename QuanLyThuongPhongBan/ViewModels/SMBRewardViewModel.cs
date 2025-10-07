using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.ClientModel.Primitives;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using static QuanLyThuongPhongBan.CLass.SearchFilter;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class SMBRewardViewModel : BaseViewModel
    {
        public ICommand? ShowAddOrEditRowCommand { get; }
        public ICommand? AddOrEditRowCommand { get; }
        public ICommand? DeleteRowCommand { get; }
        public ICommand? ReloadDataCommand { get; }
        public ICommand? CalculatorDataCommand { get; }
        public ICommand? PreviousPageCommand { get; }
        public ICommand? NextPageCommand { get; }

        private ObservableCollection<TbThuongSmb>? _list;
        public ObservableCollection<TbThuongSmb>? List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        public TbThuongSmb? TbThuongSmb { get; set; }
        public TbThuongDaiDoanSmb? TbThuongDaiDoanSmb { get; set; }
        AddEditSMBRewardWindow? aadEditSMBRewardWindow;

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

        public SMBRewardViewModel()
        {
            TbThuongDaiDoanSmb = new TbThuongDaiDoanSmb();
            TbThuongSmb = new TbThuongSmb();

            Load();

            ShowAddOrEditRowCommand = new RelayCommand<TbThuongSmb>(_ => true, ShowAddOrEditRow);
            AddOrEditRowCommand = new RelayCommand<TbThuongSmb>(_ => true, _ => AddOrEditRow());
            DeleteRowCommand = new RelayCommand<TbThuongSmb>(_ => true, DeleteRow);
            CalculatorDataCommand = new RelayCommand<TbThuongSmb>(_ => true, CalculatorData);
            ReloadDataCommand = new RelayCommand<object>(_ => true, _ => Load());
        }

        private void Load()
        {
            _ = RefreshTable();
        }

        private void ShowAddOrEditRow(TbThuongSmb model)
        {
            if (TbThuongSmb == null) return;

            if (model == null)
            {
                model = new TbThuongSmb();

                decimal[] tiLeTongSmb = { 0.03m, 0.10m, 0.10m };
                decimal[] tiLeDot1 = { 0.08m, 0.25m, 0.40m };

                for (int i = 3; i <= 5; i++)
                {
                    TbThuongDaiDoanSmb detail = new TbThuongDaiDoanSmb
                    {
                        IdPhongBan = i,
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i),
                        TiLeTongSmb = tiLeTongSmb[i - 3],
                        TiLeDot1 = tiLeDot1[i - 3]
                    };
                    model.Details.Add(detail);
                }
            }

            model.NamThuong = DateTime.Now.Date.Year.ToString();
            TbThuongSmb = model;

            aadEditSMBRewardWindow = new AddEditSMBRewardWindow();
            aadEditSMBRewardWindow.ShowDialog();
        }

        private void CalculatorData(TbThuongSmb model)
        {
            if (TbThuongSmb == null) return;

            foreach (var detail in TbThuongSmb.Details)
            {
                detail.GiaTriTongSmb = ((TbThuongSmb.TongGiaTriSmb ?? 0) * (detail.TiLeTongSmb ?? 0)) / 100;
                detail.GiaTriDot1 = ((TbThuongSmb.XuatHoaDon ?? 0) * (detail.TiLeDot1 ?? 0)) / 100;
                detail.DaThuHoiCongNo = detail.GiaTriTongSmb - detail.GiaTriDot1;
                detail.NghiemThu = detail.GiaTriDot1 + detail.DaThuHoiCongNo;

                TbThuongSmb.TongTiLeThuongSmb = TbThuongSmb.Details.Sum(x => x.TiLeTongSmb ?? 0);
                TbThuongSmb.TongGiaTriThuongSmb = TbThuongSmb.Details.Sum(x => x.GiaTriTongSmb ?? 0);
                TbThuongSmb.TongTiLeDot1 = TbThuongSmb.Details.Sum(x => x.TiLeDot1 ?? 0);
                TbThuongSmb.TongGiaTriDot1 = TbThuongSmb.Details.Sum(x => x.GiaTriDot1 ?? 0);
                TbThuongSmb.TongThuHoiCongNo = TbThuongSmb.Details.Sum(x => x.DaThuHoiCongNo ?? 0);
                TbThuongSmb.TongNghiemThu = TbThuongSmb.Details.Sum(x => x.NghiemThu ?? 0);
            }
        }

        private void AddOrEditRow()
        {
            if (TbThuongSmb == null) return;

            try
            {
                foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;

                if (TbThuongSmb.Id == 0)
                {

                    DataProvider.Ins.DB.TbThuongSmbs.Add(TbThuongSmb);

                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                        ThoiGian = DateTime.Now,
                        HanhDong = "Thêm SMB",
                        MoTa = $"Thêm mới SMB - Năm: {TbThuongSmb.NamThuong}"
                    });

                    DataProvider.Ins.DB.SaveChanges();

                    if (MessageBox.Show("Tạo mới thành công!\nBạn muốn thoát khỏi màn hình tạo mới không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                             .OfType<Window>()
                                             .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }

                    _ = RefreshTable();
                }
                else
                {
                    var existing = DataProvider.Ins.DB.TbThuongSmbs.FirstOrDefault(x => x.Id == TbThuongSmb.Id);

                    var moTa = GetChangeDescription(existing, TbThuongSmb);

                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                        ThoiGian = DateTime.Now,
                        HanhDong = "Sửa SMB",
                        MoTa = $"{GetChangeDescription(existing, TbThuongSmb)} - Năm: {TbThuongSmb.NamThuong}"
                    });

                    foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                        entry.State = EntityState.Detached;

                    DataProvider.Ins.DB.TbThuongSmbs.Update(TbThuongSmb);

                    DataProvider.Ins.DB.SaveChanges();

                    if (MessageBox.Show("Sửa thành công!\nBạn muốn thoát khỏi màn hình chinh sửa không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
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
            catch (Exception ex)
            {
                logEx(ex);
            }
        }

        string GetChangeDescription<T>(T oldObj, T newObj)
        {
            var changes = new List<string>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var oldVal = prop.GetValue(oldObj)?.ToString() ?? "";
                var newVal = prop.GetValue(newObj)?.ToString() ?? "";
                if (oldVal != newVal)
                    changes.Add($"{prop.Name}: '{oldVal}' → '{newVal}'");
            }
            return string.Join(", ", changes);
        }

        private void DeleteRow(TbThuongSmb model)
        {
            try
            {
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?", "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;

                TbThuongSmb = model;

                foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;

                DataProvider.Ins.DB.TbThuongSmbs.Remove(TbThuongSmb);

                DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                {
                    IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                    ThoiGian = DateTime.Now,
                    HanhDong = "Xoá SMB",
                    MoTa = $"Xoá SMB - Năm: {TbThuongSmb.NamThuong}"
                });

                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Xoá thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

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
            catch (InvalidOperationException)
            {
                MessageBox.Show("Lỗi thực hiện thao tác" , "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

                await _dbLock.WaitAsync(); // 🔒 Chặn các truy vấn song song
                try
                {
                    // Truy vấn dữ liệu chính
                    var query = DataProvider.Ins.DB.TbThuongSmbs
                        .AsNoTracking()
                        .AsQueryable();

                    // Truy vấn dữ liệu chi tiết
                    var queryDetails = await DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();


                    // Áp dụng tìm kiếm trực tiếp trên cơ sở dữ liệu
                    query = SearchViewModel.Search(query, SearchFill ?? string.Empty);


                    if (List == null)
                        List = new ObservableCollection<TbThuongSmb>();
                    List.Clear();

                    foreach (var item in query)
                    {
                        var details = queryDetails
                            .Where(d => d.IdThuongSmb == item.Id)
                            .Select(d => new TbThuongDaiDoanSmb
                            {
                                Id = d.Id,
                                IdPhongBan = d.IdPhongBan,
                                IdThuongSmb = d.IdThuongSmb,
                                TiLeTongSmb = d.TiLeTongSmb,
                                GiaTriTongSmb = d.GiaTriTongSmb,
                                TiLeDot1 = d.TiLeDot1,
                                GiaTriDot1 = d.GiaTriDot1,
                                ThuHoiCongNo = d.ThuHoiCongNo,
                                NghiemThu = d.NghiemThu,
                                IdPhongBanNavigation = d.IdPhongBanNavigation
                            });

                        foreach (var ct in details)
                        {
                            item.Details.Add(ct);
                        }

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
