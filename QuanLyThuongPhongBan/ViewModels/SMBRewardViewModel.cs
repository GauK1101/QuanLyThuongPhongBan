using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using static QuanLyThuongPhongBan.CLass.SearchFilter;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class SMBRewardViewModel : BaseViewModel
    {
        #region Commands

        public ICommand? ShowAddOrEditRowCommand { get; }
        public ICommand? AddOrEditRowCommand { get; }
        public ICommand? DeleteRowCommand { get; }
        public ICommand? ReloadDataCommand { get; }
        public ICommand? CalculatorDataCommand { get; }
        public ICommand? PreviousPageCommand { get; }
        public ICommand? NextPageCommand { get; }

        #endregion

        #region Properties - Dữ liệu chính

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

        private ObservableCollection<TbThuHoiCongNoSmbTheoThang>? _listMonth;
        public ObservableCollection<TbThuHoiCongNoSmbTheoThang>? ListMonth
        {
            get => _listMonth;
            set
            {
                _listMonth = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties - Model hiện tại

        public TbThuongSmb? TbThuongSmb { get; set; }
        public TbThuongDaiDoanSmb? TbThuongDaiDoanSmb { get; set; }
        AddEditSMBRewardWindow? aadEditSMBRewardWindow;

        #endregion

        #region Search & Pagination

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

        #endregion

        #region Constructor

        public SMBRewardViewModel()
        {
            // Khởi tạo model để tránh lỗi null
            TbThuongDaiDoanSmb = new TbThuongDaiDoanSmb();
            TbThuongSmb = new TbThuongSmb();

            Load(); // Load dữ liệu ban đầu

            // Command binding cho View
            ShowAddOrEditRowCommand = new RelayCommand<TbThuongSmb>(_ => true, ShowAddOrEditRow);
            AddOrEditRowCommand = new RelayCommand<TbThuongSmb>(_ => true, _ => AddOrEditRow());
            DeleteRowCommand = new RelayCommand<TbThuongSmb>(_ => true, DeleteRow);
            CalculatorDataCommand = new RelayCommand<TbThuongSmb>(_ => true, _ => CalculatorData());
            ReloadDataCommand = new RelayCommand<object>(_ => true, _ => Load());
        }

        private void Load()
        {
            _ = RefreshTable();
        }

        // Hàm hiển thị cửa sổ thêm hoặc sửa bản ghi thưởng SMB
        private void ShowAddOrEditRow(TbThuongSmb model)
        {
            // Nếu biến chính đang null thì không làm gì cả (bảo vệ)
            if (TbThuongSmb == null) return;

            // Nếu model truyền vào là null => tạo mới một bản ghi thưởng SMB
            if (model == null)
            {
                model = new TbThuongSmb(); // Tạo mới object

                // Các tỷ lệ SMB tổng tương ứng cho các phòng ban 3, 4, 5
                decimal[] tiLeTongSmb = { 0.08m, 0.25m, 0.40m };

                // Các tỷ lệ đợt 1 tương ứng cho các phòng ban 3, 4, 5
                decimal[] tiLeDot1 = { 0.03m, 0.10m, 0.10m };

                // Lặp qua các phòng ban có ID từ 3 đến 5
                for (int i = 3; i <= 5; i++)
                {
                    // Tạo mới bản ghi chi tiết thưởng đại đoàn SMB cho mỗi phòng ban
                    TbThuongDaiDoanSmb detail = new TbThuongDaiDoanSmb
                    {
                        IdPhongBan = i, // Gán ID phòng ban
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i), // Load thông tin điều hướng phòng ban
                        TiLeTongSmb = tiLeTongSmb[i - 3], // Gán tỷ lệ tổng SMB theo thứ tự
                        TiLeDot1 = tiLeDot1[i - 3] // Gán tỷ lệ đợt 1 theo thứ tự
                    };

                    // Thêm chi tiết này vào danh sách Details của model
                    model.TbThuongDaiDoanSmbs.Add(detail);
                }

                // Gán giá trị chuỗi thể hiện quý và năm hiện tại cho thuộc tính QuyNamThuong.
                model.QuyNamThuong = $"Q{(DateTime.Now.Month - 1) / 3 + 1} - {DateTime.Now.Year}";
            }

            // Gán model vào biến toàn cục để lưu trạng thá`i
            TbThuongSmb = model;

            // Mở cửa sổ thêm/sửa (Add/Edit) thưởng SMB
            aadEditSMBRewardWindow = new AddEditSMBRewardWindow();
            aadEditSMBRewardWindow.ShowDialog(); // Hiển thị dialog để nhập liệu
        }

        // Hàm tính toán lại các giá trị thưởng SMB dựa trên tỷ lệ và dữ liệu đầu vào
        private void CalculatorData()
        {
            // Nếu model chính đang null thì thoát ra (không làm gì)
            if (TbThuongSmb == null) return;

            // Lặp qua từng chi tiết thưởng trong danh sách
            foreach (var detail in TbThuongSmb.TbThuongDaiDoanSmbs)
            {
                // Tính giá trị tổng SMB cho chi tiết = (Tổng giá trị SMB * Tỷ lệ thưởng) / 100
                detail.GiaTriTongSmb = ((TbThuongSmb.TongGiaTriSmb ?? 0) * (detail.TiLeTongSmb ?? 0)) / 100;

                // Tính giá trị đợt 1 = (Giá trị xuất hóa đơn * Tỷ lệ đợt 1) / 100
                detail.GiaTriDot1 = ((TbThuongSmb.XuatHoaDon ?? 0) * (detail.TiLeDot1 ?? 0)) / 100;

                // Thu hồi công nợ = Tổng SMB - Đợt 1
                detail.ThuHoiCongNo = detail.GiaTriTongSmb - detail.GiaTriDot1;

                // Nghiệm thu = Đợt 1 + Thu hồi công nợ
                detail.NghiemThu = detail.GiaTriDot1 + detail.ThuHoiCongNo;

                // Tỉ lệ thu hồi công nợ = Tỉ lệ tổng smb + Tỉ lệ đợt 1
                detail.TiLeThuHoiCongNo = detail.TiLeTongSmb - detail.TiLeDot1;

                // Cập nhật tổng tỷ lệ thưởng SMB (cộng tất cả tỷ lệ từ chi tiết)
                TbThuongSmb.TongTiLeThuongSmb = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.TiLeTongSmb ?? 0);

                // Cập nhật tổng giá trị thưởng SMB
                TbThuongSmb.TongGiaTriThuongSmb = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.GiaTriTongSmb ?? 0);

                // Cập nhật tổng tỷ lệ đợt 1
                TbThuongSmb.TongTiLeDot1 = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.TiLeDot1 ?? 0);

                // Cập nhật tổng giá trị đợt 1
                TbThuongSmb.TongGiaTriDot1 = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.GiaTriDot1 ?? 0);

                // Cập nhật tổng thu hồi công nợ
                TbThuongSmb.TongThuHoiCongNo = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.ThuHoiCongNo ?? 0);

                // Cập nhật tổng nghiệm thu
                TbThuongSmb.TongNghiemThu = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(x => x.NghiemThu ?? 0);
            }
        }

        // Hàm thêm mới hoặc chỉnh sửa một bản ghi thưởng SMB
        private void AddOrEditRow()
        {
            // Nếu đối tượng thưởng đang làm việc là null thì thoát khỏi hàm
            if (TbThuongSmb == null) return;

            try
            {
                // Trường hợp thêm mới
                if (TbThuongSmb.Id == 0)
                {
                    // Thêm bản ghi thưởng SMB vào DbSet
                    DataProvider.Ins.DB.TbThuongSmbs.Add(TbThuongSmb);

                    UpdateMonthlyRecord();

                    // Ghi log thao tác "Thêm SMB"
                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0], // Lấy ID người dùng từ setting
                        ThoiGian = DateTime.Now, // Ghi lại thời gian thực hiện
                        HanhDong = "Thêm SMB", // Hành động thực hiện
                        MoTa = $"Thêm mới SMB - Năm: {TbThuongSmb.QuyNamThuong}" // Mô tả thêm
                    });

                    // Gỡ liên kết theo dõi trạng thái entity để tránh lỗi khi cập nhật
                    //foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    //    entry.State = EntityState.Detached;

                    // Lưu thay đổi vào database
                    DataProvider.Ins.DB.SaveChanges();

                    // Thông báo tạo mới thành công, hỏi người dùng có muốn thoát màn hình không
                    if (MessageBox.Show("Tạo mới thành công!\nBạn muốn thoát khỏi màn hình tạo mới không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        // Lấy cửa sổ hiện tại và đóng nó
                        var currentWindow = Application.Current.Windows
                                                 .OfType<Window>()
                                                 .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }

                    // Làm mới lại bảng dữ liệu hiển thị (async)
                    _ = RefreshTable();
                }
                // Trường hợp sửa bản ghi cũ
                else
                {
                    // Tìm bản ghi SMB cũ trong database
                    var existing = DataProvider.Ins.DB.TbThuongSmbs.FirstOrDefault(x => x.Id == TbThuongSmb.Id);

                    // Gọi hàm cập nhật vào bảng theo tháng (dựa theo chênh lệch)
                    UpdateMonthlyRecord();

                    // Ghi log thao tác "Sửa SMB"
                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0], // Lấy ID người dùng từ setting
                        ThoiGian = DateTime.Now, // Ghi lại thời gian thực hiện
                        HanhDong = "Sửa SMB", // Hành động thực hiện
                        MoTa = $"{GetChangeDescription(existing, TbThuongSmb)} - Năm: {TbThuongSmb.QuyNamThuong}" // Mô tả thay đổi
                    });

                    // Gỡ theo dõi trạng thái để cập nhật lại object
                    //foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    //    entry.State = EntityState.Detached;

                    // Cập nhật lại bản ghi SMB trong DB
                    DataProvider.Ins.DB.TbThuongSmbs.Update(TbThuongSmb);

                    // Lưu thay đổi vào database
                    DataProvider.Ins.DB.SaveChanges();

                    // Thông báo sửa thành công, hỏi người dùng có muốn thoát màn hình không
                    if (MessageBox.Show("Sửa thành công!\nBạn muốn thoát khỏi màn hình chỉnh sửa không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                                 .OfType<Window>()
                                                 .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }
                }
            }
            // Xử lý lỗi cập nhật DB
            catch (DbUpdateException dbUpdateEx)
            {
                string errorMessage;

                // Nếu là lỗi SQL cụ thể thì xử lý theo mã lỗi
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

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Các lỗi không xác định khác
                    MessageBox.Show("Lỗi không xác định trong quá trình cập nhật dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            // Xử lý lỗi chung ngoài lỗi DB
            catch (Exception ex)
            {
                logEx(ex); // Ghi log lỗi hoặc hiện thông báo
            }
        }

        // Cập nhật giá trị công nợ vào bảng theo tháng hiện tại
        private void UpdateMonthlyRecord()
        {
            // Nếu đối tượng gốc null thì thoát sớm
            if (TbThuongSmb == null) return;

            // Lấy toàn bộ bản ghi cũ từ DB theo IdThuongSmb (không tracking để không dính context)
            var existing = DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                .AsNoTracking()
                .Where(x => x.IdThuongSmb == TbThuongSmb.Id)
                .ToList();

            // Tạo dictionary để tra cứu nhanh theo Id: { Id => ThuHoiCongNo }
            var oldDict = existing.ToDictionary(x => x.Id, x => x.ThuHoiCongNo ?? 0);

            // Tính tổng thay đổi giữa dữ liệu mới và cũ
            // Với mỗi item: (giá trị mới - giá trị cũ)
            decimal? totalDifference = TbThuongSmb.TbThuongDaiDoanSmbs.Sum(newItem =>
            {
                decimal newValue = newItem.ThuHoiCongNo ?? 0;
                decimal oldValue = oldDict.TryGetValue(newItem.Id, out var val) ? val : 0;

                return newValue - oldValue; // Tổng thay đổi
            });

            // Lấy tháng và năm hiện tại
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            // Biến tên property theo format: "Thang1", "Thang2", ...
            var propertyName = $"Thang{month}";

            // Tìm bản ghi tổng công nợ theo tháng cho năm hiện tại
            var existingDate = DataProvider.Ins.DB.TbThuHoiCongNoSmbTheoThangs
                .FirstOrDefault(x => x.Nam == year);

            // Lấy thông tin property động theo tên tháng (sử dụng Reflection)
            var property = typeof(TbThuHoiCongNoSmbTheoThang).GetProperty(propertyName);

            // Nếu tồn tại property và có thể gán giá trị
            if (property != null && property.CanWrite)
            {
                if (existingDate != null)
                {
                    // Nếu đã có bản ghi của năm hiện tại
                    // => cộng giá trị thay đổi vào tháng tương ứng
                    var currentValue = property.GetValue(existingDate) as decimal?;
                    var newValue = currentValue.GetValueOrDefault() + (totalDifference ?? 0);
                    property.SetValue(existingDate, newValue);
                }
                else
                {
                    // Nếu chưa có bản ghi năm => tạo mới
                    var newRecord = new TbThuHoiCongNoSmbTheoThang
                    {
                        Nam = year
                    };

                    // Gán giá trị cho tháng hiện tại
                    property.SetValue(newRecord, totalDifference ?? 0);

                    // Thêm bản ghi vào DB
                    DataProvider.Ins.DB.TbThuHoiCongNoSmbTheoThangs.Add(newRecord);
                }

                // Lưu thay đổi xuống DB
                DataProvider.Ins.DB.SaveChanges();
            }
        }

        // Chỉ so sánh các property đơn giản (kiểu giá trị và string).
        string GetChangeDescription<T>(T oldObj, T newObj)
        {
            // Danh sách chứa các mô tả thay đổi
            var changes = new List<string>();

            // Duyệt qua tất cả các property của kiểu T
            foreach (var prop in typeof(T).GetProperties())
            {
                // Bỏ qua property nếu không đọc được
                if (!prop.CanRead)
                    continue;

                // Bỏ qua các property là class (đối tượng phức tạp), trừ string (vì string là class nhưng xử lý như giá trị)
                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    continue;

                // Lấy giá trị của property trong cả 2 object
                var oldVal = prop.GetValue(oldObj);
                var newVal = prop.GetValue(newObj);

                // So sánh giá trị thật (object.Equals) thay vì so sánh chuỗi
                if (!object.Equals(oldVal, newVal))
                {
                    // Chuyển giá trị về chuỗi để hiển thị; nếu null thì hiển thị là "null"
                    string oldStr = oldVal?.ToString() ?? "null";
                    string newStr = newVal?.ToString() ?? "null";

                    // Thêm vào danh sách thay đổi theo format: "PropertyName: 'oldValue' → 'newValue'"
                    changes.Add($"{prop.Name}: '{oldStr}' → '{newStr}'");
                }
            }

            // Gộp các thay đổi thành một chuỗi, phân cách bằng dấu phẩy
            return string.Join(", ", changes);
        }

        // Hàm thực hiện xóa một bản ghi TbThuongSmb được truyền vào
        private void DeleteRow(TbThuongSmb model)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận xóa
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?", "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                    return; // Nếu chọn Cancel thì thoát hàm

                // Gán đối tượng được chọn vào biến xử lý
                TbThuongSmb = model;

                // Detach toàn bộ entity đang được EF theo dõi để tránh xung đột trạng thái
                foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;

                // Đánh dấu đối tượng cần xóa
                DataProvider.Ins.DB.TbThuongSmbs.Remove(TbThuongSmb);

                // Ghi nhật ký hành động xóa
                DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                {
                    IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0], // Lấy ID tài khoản từ cấu hình
                    ThoiGian = DateTime.Now,
                    HanhDong = "Xoá SMB",
                    MoTa = $"Xoá SMB - Năm: {TbThuongSmb.QuyNamThuong}"
                });

                // Thực hiện lưu thay đổi vào CSDL
                DataProvider.Ins.DB.SaveChanges();

                // Thông báo xóa thành công
                MessageBox.Show("Xoá thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Làm mới lại bảng dữ liệu hiển thị
                _ = RefreshTable();
            }
            catch (DbUpdateException dbUpdateEx)
            {
                string errorMessage;

                // Kiểm tra lỗi cụ thể nếu là lỗi từ SQL Server
                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 547: // Lỗi ràng buộc khóa ngoại (ví dụ: bản ghi đang được dùng ở bảng khác)
                            errorMessage = "Không thể xóa vì có ràng buộc ngoại. Xin kiểm tra dữ liệu liên quan.";
                            break;
                        default:
                            errorMessage = "Lỗi không xác định: " + sqlEx.Message;
                            break;
                    }

                    // Hiển thị lỗi nếu có thông báo cụ thể
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Nếu lỗi không phải do SQL
                    MessageBox.Show("Lỗi không xác định trong quá trình xóa dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException)
            {
                // Bắt lỗi thao tác không hợp lệ (thường là do state không đúng)
                MessageBox.Show("Lỗi thực hiện thao tác", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Bắt tất cả các lỗi khác và log lại
                logEx(ex);
            }
        }

        // Biến dùng để hủy tác vụ đang chạy (nếu có) khi cần khởi chạy tác vụ mới
        private CancellationTokenSource? cancellationTokenSource;

        // Hàm để hủy tác vụ trước đó nếu đang chạy rồi tạo CancellationTokenSource mới
        private void CancelPreviousTask()
        {
            if (cancellationTokenSource != null)
            {
                // Yêu cầu hủy tác vụ đang chạy
                cancellationTokenSource.Cancel();

                // Giải phóng tài nguyên của cancellationTokenSource cũ
                cancellationTokenSource.Dispose();
            }

            // Tạo mới CancellationTokenSource để dùng cho tác vụ mới
            cancellationTokenSource = new CancellationTokenSource();
        }

        // SemaphoreSlim để khóa đồng bộ truy cập tài nguyên (ở đây có thể dùng cho DB hoặc code critical section)
        // Chỉ cho phép 1 luồng vào cùng lúc (bảo vệ tránh race condition)
        private static readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1, 1);

        private async Task RefreshTable()
        {
            // Hủy tác vụ trước đó nếu có, để tránh chạy chồng tác vụ
            CancelPreviousTask();

            try
            {
                // Nếu cancellationTokenSource đã được tạo thì chờ 300ms
                // và có thể bị hủy nếu tác vụ khác gọi CancelPreviousTask
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                // Khóa đồng bộ tránh chạy song song các truy vấn DB gây lỗi hoặc tranh chấp dữ liệu
                await _dbLock.WaitAsync(); // 🔒 Chặn các truy vấn song song

                try
                {
                    // Truy vấn dữ liệu chính (TbThuongSmbs) không theo dõi thay đổi (AsNoTracking)
                    var query = DataProvider.Ins.DB.TbThuongSmbs
                        .AsNoTracking()
                        .AsQueryable();

                    // Truy vấn dữ liệu chi tiết (TbThuongDaiDoanSmbs) kèm theo dữ liệu phòng ban (Include)
                    var queryDaiDoan = await DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();

                    // Truy vấn dữ liệu tháng (TbThuHoiCongNoSmbTheoThang) thành ObservableCollection cho binding UI
                    ListMonth = new ObservableCollection<TbThuHoiCongNoSmbTheoThang>(
                        await DataProvider.Ins.DB.TbThuHoiCongNoSmbTheoThangs
                        .AsNoTracking()
                        .ToListAsync());

                    // Áp dụng bộ lọc tìm kiếm lên truy vấn dữ liệu chính
                    query = SearchViewModel.Search(query, SearchFill ?? string.Empty);

                    // Nếu List null thì khởi tạo mới
                    if (List == null)
                        List = new ObservableCollection<TbThuongSmb>();

                    // Xóa hết các phần tử cũ trước khi thêm mới
                    List.Clear();

                    // Duyệt từng bản ghi chính trong truy vấn
                    foreach (var item in query)
                    {
                        // Lấy chi tiết tương ứng theo IdThuongSmb
                        var tbThuongDaiDoanDuAn = queryDaiDoan
                            .Where(d => d.IdThuongSmb == item.Id)
                            .ToList();

                        // Thêm từng chi tiết vào chi tiết của bản ghi chính
                        foreach (var ct in tbThuongDaiDoanDuAn)
                            item.TbThuongDaiDoanSmbs.Add(ct);

                        // Thêm bản ghi chính (kèm chi tiết) vào List dùng binding UI
                        List.Add(item);
                    }
                }
                finally
                {
                    // Giải phóng khóa để cho phép truy vấn khác vào DB
                    _dbLock.Release(); // 🔓 Giải phóng lock
                }
            }
            catch (InvalidOperationException)
            {
                // Bỏ qua lỗi liên quan đến DbContext chạy nhiều lệnh cùng lúc
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua lỗi tác vụ bị hủy khi gọi CancelPreviousTask
            }
            catch (Exception ex)
            {
                // Hiện thông báo lỗi cho người dùng nếu có lỗi khác xảy ra
                MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Hàm ghi log lỗi ra file và hiển thị thông báo lỗi
        private void logEx(Exception ex)
        {
            // Đường dẫn thư mục lưu file log (cùng thư mục chạy app, thư mục Logs)
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            // Tạo thư mục Logs nếu chưa tồn tại
            Directory.CreateDirectory(logDir);

            // Đường dẫn file log theo ngày hiện tại (error_yyyyMMdd.txt)
            string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.txt");

            // Nội dung log gồm thời gian, thông báo lỗi và stack trace, kèm phân cách
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}\n{ex.StackTrace}\n----------------------------------------\n";

            // Ghi nội dung log vào file, nếu file chưa có sẽ tạo mới, nếu có sẽ nối thêm vào
            File.AppendAllText(logFile, logMessage);

            // Hiển thị thông báo lỗi cho người dùng biết rằng đã có lỗi xảy ra
            MessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file log để biết thêm chi tiết.",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}
