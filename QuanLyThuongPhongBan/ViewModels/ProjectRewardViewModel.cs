using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.ModelSettings;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class ProjectRewardViewModel : BaseViewModel
    {
        #region Commands
        // Các lệnh (command) được binding từ UI, dùng để thao tác dữ liệu
        public ICommand? ShowAddOrEditRowCommand { get; }
        public ICommand? AddOrEditRowCommand { get; }
        public ICommand? DeleteRowCommand { get; }
        public ICommand? ReloadDataCommand { get; }
        public ICommand? CalculatorDataCommand { get; }
        public ICommand? CalculatedBonusCommand { get; }
        public ICommand? PreviousPageCommand { get; }
        public ICommand? NextPageCommand { get; }
        public ICommand? CloseCommand { get; }
        public ICommand? DeleteDetailCommand { get; }
        #endregion

        #region Properties
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

        // Đối tượng đang được chọn hoặc chỉnh sửa
        public TbThuongDuAn? TbThuongDuAn { get; set; }

        // Đối tượng đang được chọn hoặc chỉnh sửa
        private ObservableCollection<TbThuongDuAn>? _backupList;

        // Thông tin đại đoàn/dự án liên quan
        public TbThuongDaiDoanDuAn? TbThuongDaiDoanDuAn { get; set; }

        // Cửa sổ thêm/sửa (dialog)
        AddEditProjectRewardWindow? addEditProjectRewardWindow;
        #endregion

        #region Search & Pagination
        public AppSettings Settings => App.Settings;

        private string? _searchFill = string.Empty;
        public string? SearchFill
        {
            get => _searchFill;
            set
            {
                _searchFill = value;
                OnPropertyChanged();
                _ = RefreshTable(); // Gọi làm mới bảng khi người dùng nhập tìm kiếm
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
        #endregion

        #region Constructor
        public ProjectRewardViewModel()
        {
            TbThuongDaiDoanDuAn = new TbThuongDaiDoanDuAn();
            TbThuongDuAn = new TbThuongDuAn();

            Load(); // Nạp dữ liệu ban đầu

            // Gán các command với hàm xử lý tương ứng
            ShowAddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, ShowAddOrEditRow);
            AddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, _ => AddOrEditRow());
            DeleteRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, DeleteRow);
            CalculatorDataCommand = new RelayCommand<TbThuongDuAn>(_ => true, _ => CalculatorData());
            CalculatedBonusCommand = new RelayCommand<TbThuongDuAn>(_ => true, _ => CalculatedBonus());
            ReloadDataCommand = new RelayCommand<object>(_ => true, _ => Load());
            CloseCommand = new RelayCommand<Window>(_ => true, CloseWindow);
            DeleteDetailCommand = new RelayCommand<TbThuongDuAnChiTiet>(_ => true, DeleteDetail);
        }
        #endregion

        #region Load dữ liệu
        /// <summary>
        /// Gọi hàm RefreshTable() để tải lại danh sách dữ liệu.
        /// </summary>
        private void Load()
        {
            _ = RefreshTable();
        }
        #endregion

        #region Hiển thị cửa sổ Thêm/Sửa thưởng dự án
        /// <summary>
        /// Mở cửa sổ thêm hoặc sửa một bản ghi thưởng dự án.
        /// Nếu model == null thì tạo dữ liệu mẫu mặc định.
        /// </summary>
        private void ShowAddOrEditRow(TbThuongDuAn model)
        {
            if (TbThuongDuAn == null) return;

            // Nếu người dùng bấm “Thêm mới”
            if (model == null)
            {
                model = new TbThuongDuAn();

                // Tỷ lệ mặc định cho 5 phòng ban
                decimal[] tongTiLeThuongDuAn = { 0.5m, 0.25m, 0.07m, 0.25m, 0.5m };
                decimal[] tiLeDieuChinhDot1 = { 0.025m, 0.015m, 0, 0, 0 };
                decimal[] tiLeDieuChinhDot2 = { 0.125m, 0.025m, 0.025m, 0.05m, 0.1m };

                // Khởi tạo chi tiết thưởng đại đoàn cho từng phòng ban
                for (int i = 1; i <= 5; i++)
                {
                    TbThuongDaiDoanDuAn item = new TbThuongDaiDoanDuAn
                    {
                        IdPhongBan = i,
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i),
                        TiLeTongGoi = tongTiLeThuongDuAn[i - 1],
                        TiLeDieuChinhDot1 = tiLeDieuChinhDot1[i - 1],
                        TiLeDieuChinhDot2 = tiLeDieuChinhDot2[i - 1]
                    };
                    model.TbThuongDaiDoanDuAns.Add(item);
                }

                // Mặc định năm thưởng là năm hiện tại
                model.NamThuong = DateTime.Now.Year.ToString();

                // Tổng các tỷ lệ
                model.TongTiLeThuongDuAn = model.TbThuongDaiDoanDuAns.Sum(x => x.TiLeTongGoi);
                model.TongTiLeDieuChinhDot1 = model.TbThuongDaiDoanDuAns.Sum(x => x.TiLeDieuChinhDot1);
                model.TongTiLeDieuChinhDot2 = model.TbThuongDaiDoanDuAns.Sum(x => x.TiLeDieuChinhDot2);
            }

            TbThuongDuAn = model;
            // Tạo backup (clone, không reference chung)
            _backupList = JsonConvert.DeserializeObject<ObservableCollection<TbThuongDuAn>>(
                JsonConvert.SerializeObject(List, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                })
            );

            // Hiển thị cửa sổ thêm/sửa
            addEditProjectRewardWindow = new AddEditProjectRewardWindow();
            addEditProjectRewardWindow.ShowDialog();
        }
        #endregion

        #region Tính toán dữ liệu thưởng
        /// <summary>
        /// Tính toán các giá trị thưởng và điều chỉnh dựa trên dữ liệu chi tiết.
        /// </summary>
        private void CalculatorData()
        {
            if (TbThuongDuAn == null) return;

            // --- Tính tổng hợp cho từng đại đoàn ---
            foreach (var item in TbThuongDuAn.TbThuongDaiDoanDuAns)
            {
                item.GiaTriTongGoi = (TbThuongDuAn.QuyetToan * item.TiLeTongGoi) / 100;
                item.GiaTriDieuChinhDot1 = (TbThuongDuAn.GiaTriHopDong * item.TiLeDieuChinhDot1) / 100;
                item.GiaTriDieuChinhDot2 = (TbThuongDuAn.GiaTriHopDong * item.TiLeDieuChinhDot2) / 100;

                item.ThuHoiCongNo = item.GiaTriTongGoi - item.GiaTriDieuChinhDot1 - item.GiaTriDieuChinhDot2;
                item.NghiemThu = item.GiaTriDieuChinhDot1 + item.GiaTriDieuChinhDot2 + item.ThuHoiCongNo;
            }

            // --- Tổng hợp lại các chỉ tiêu chung ---
            TbThuongDuAn.TongTiLeThuongDuAn = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeTongGoi);
            TbThuongDuAn.TongGiaTriThuongDuAn = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriTongGoi);
            TbThuongDuAn.TongTiLeDieuChinhDot1 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeDieuChinhDot1);
            TbThuongDuAn.TongGiaTriDieuChinhDot1 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriDieuChinhDot1);
            TbThuongDuAn.TongTiLeDieuChinhDot2 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.TiLeDieuChinhDot2);
            TbThuongDuAn.TongGiaTriDieuChinhDot2 = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.GiaTriDieuChinhDot2);
            TbThuongDuAn.TongThuHoiCongNo = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.ThuHoiCongNo);
            TbThuongDuAn.TongNghiemThu = TbThuongDuAn.TbThuongDaiDoanDuAns.Sum(d => d.NghiemThu);

            // --- Cập nhật tổng hợp doanh thu ---
            TbThuongDuAn.GiaTriHopDong = TbThuongDuAn.TbThuongDuAnChiTiets.Sum(d => d.DoanhThuHopDong);
            TbThuongDuAn.QuyetToan = TbThuongDuAn.TbThuongDuAnChiTiets.Sum(d => d.DoanhThuQuyetToan);

            // --- Phân bổ doanh thu theo phòng ban ---
            var filterByDepartment = new Dictionary<int, Func<TbThuongDuAnChiTiet, bool>>
            {
                { 1, d => d.Tvgp > 0 },
                { 2, d => d.Hsda > 0 },
                { 3, d => d.Po > 0 },
                { 4, d => d.Ktk > 0 },
                { 5, d => d.Ttdvkt > 0 }
            };

            foreach (var item in TbThuongDuAn.TbThuongDaiDoanDuAns)
            {
                if (filterByDepartment.TryGetValue(item.IdPhongBan, out var condition)){
                    item.DoanhThuHopDong = TbThuongDuAn.TbThuongDuAnChiTiets.Where(condition).Sum(d => d.DoanhThuHopDong ?? 0m);
                    item.DoanhThuXuatHoaDon = TbThuongDuAn.TbThuongDuAnChiTiets.Where(condition).Sum(d => d.DoanhThuQuyetToan ?? 0m);
                }
                else
                    item.DoanhThuHopDong = 0m;
            }
        }

        /// <summary>
        /// Hàm phụ: Lấy tỷ lệ tổng gói theo Id phòng ban.
        /// </summary>
        private decimal GetTiLe(int idPhongBan)
        {
            return TbThuongDuAn?.TbThuongDaiDoanDuAns
                .FirstOrDefault(x => x.IdPhongBan == idPhongBan)?.TiLeTongGoi ?? 0m;
        }
        #endregion

        #region Tính thưởng theo giai đoạn
        /// <summary>
        /// Tính thưởng theo từng giai đoạn, dựa trên doanh thu từng phòng ban.
        /// </summary>
        private void CalculatedBonus()
        {
            if (TbThuongDuAn == null) return;

            // --- Cập nhật chi tiết theo từng dòng dự án ---
            foreach (var item in TbThuongDuAn.TbThuongDuAnChiTiets)
            {
                item.DoanhThuChuaXuatHoaDon = (item.DoanhThuQuyetToan ?? 0) - (item.DoanhThuDaXuatHoaDon ?? 0);
                item.ChuaThanhToan = (item.DoanhThuQuyetToan ?? 0) - (item.DaThanhToan ?? 0);

                // Tính phần thưởng theo từng phòng ban (1–5)
                item.Tvgp = Math.Round((item.DoanhThuQuyetToan ?? 0m) * GetTiLe(1) / 100m, 0);
                item.Hsda = Math.Round((item.DoanhThuQuyetToan ?? 0m) * GetTiLe(2) / 100m, 0);
                item.Po = Math.Round((item.DoanhThuQuyetToan ?? 0m) * GetTiLe(3) / 100m, 0);
                item.Ktk = Math.Round((item.DoanhThuQuyetToan ?? 0m) * GetTiLe(4) / 100m, 0);
                item.Ttdvkt = Math.Round((item.DoanhThuQuyetToan ?? 0m) * GetTiLe(5) / 100m, 0);
            }
        }
        #endregion
        #region 📝 Thêm hoặc sửa thưởng dự án
        /// <summary>
        /// Lưu thông tin thưởng dự án — nếu Id = 0 thì thêm mới, ngược lại thì cập nhật.
        /// Có ghi nhật ký thao tác (TbNhatKy) và xử lý lỗi khi lưu.
        /// </summary>
        private void AddOrEditRow()
        {
            if (TbThuongDuAn == null) return;

            try
            {
                // Gỡ theo dõi tất cả entity cũ để tránh lỗi “entity already being tracked”
                foreach (var entry in DataProvider.Ins.DB.ChangeTracker.Entries().ToList())
                    entry.State = EntityState.Detached;

                if (TbThuongDuAn.Id == 0) // Thêm mới
                {
                    var invalidItems = TbThuongDuAn.TbThuongDuAnChiTiets
                                        .Where(x => string.IsNullOrWhiteSpace(x.ChuDauTu)
                                                 || string.IsNullOrWhiteSpace(x.HopDongSo))
                                        .ToList();

                    // Xóa chúng khỏi danh sách chính
                    foreach (var item in invalidItems)
                    {
                        TbThuongDuAn.TbThuongDuAnChiTiets.Remove(item);
                    }

                    foreach (var chiTiet in TbThuongDuAn.TbThuongDaiDoanDuAns)
                    {
                        chiTiet.IdPhongBanNavigation = null; // tránh EF insert
                    }

                    // Bây giờ add TbThuongDuAn
                    DataProvider.Ins.DB.TbThuongDuAns.Add(TbThuongDuAn);

                    // Ghi lại nhật ký thao tác
                    DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                    {
                        IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                        ThoiGian = DateTime.Now,
                        HanhDong = "Thêm thưởng dự án",
                        MoTa = $"Thêm mới thưởng dự án - Năm: {TbThuongDuAn.NamThuong}"
                    });

                    DataProvider.Ins.DB.SaveChanges();

                    // Thông báo thành công và hỏi người dùng có muốn quay lại không
                    if (MessageBox.Show("Tạo mới thành công!\nBạn muốn về màn hình chính không ?",
                        "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                             .OfType<Window>()
                                             .SingleOrDefault(w => w.IsActive);
                        currentWindow?.Close();
                    }
                }
                else // Cập nhật
                {
                    var invalidItems = TbThuongDuAn.TbThuongDuAnChiTiets
                                        .Where(x => string.IsNullOrWhiteSpace(x.ChuDauTu)
                                                 || string.IsNullOrWhiteSpace(x.HopDongSo))
                                        .ToList();

                    // Xóa chúng khỏi danh sách chính
                    foreach (var item in invalidItems)
                    {
                        TbThuongDuAn.TbThuongDuAnChiTiets.Remove(item);
                    }

                    DataProvider.Ins.DB.TbThuongDuAns.Update(TbThuongDuAn);
                    DataProvider.Ins.DB.SaveChanges();

                    if (MessageBox.Show("Sửa thành công!\nBạn muốn về màn hình chính không ?",
                        "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
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
                // Xử lý các lỗi database thường gặp
                string errorMessage;

                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 2627: // Trùng khóa chính
                            errorMessage = "Trùng khóa chính. Dữ liệu đã tồn tại.";
                            break;
                        case 547: // Vi phạm ràng buộc ngoại
                            errorMessage = "Vi phạm ràng buộc dữ liệu. Vui lòng kiểm tra lại.";
                            break;
                        default:
                            errorMessage = "Lỗi không xác định: " + sqlEx.Message;
                            break;
                    }

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi không xác định trong quá trình cập nhật dữ liệu.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Lỗi thực hiện thao tác. Vui lòng thử lại.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logEx(ex); // Hàm ghi log ngoại lệ nội bộ
            }
        }
        #endregion

        #region 🗑️ Xóa thưởng dự án
        /// <summary>
        /// Xóa bản ghi thưởng dự án được chọn. Có xác nhận, ghi nhật ký và xử lý lỗi ràng buộc.
        /// </summary>
        private void DeleteRow(TbThuongDuAn model)
        {
            try
            {
                // Hỏi người dùng xác nhận xóa
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?",
                    "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                    return;

                // Ghi lại nhật ký xóa
                DataProvider.Ins.DB.TbNhatKies.Add(new TbNhatKy
                {
                    IdTaiKhoan = Properties.Settings.Default.User.Split('|')[0],
                    ThoiGian = DateTime.Now,
                    HanhDong = "Xoá thưởng dự án",
                    MoTa = $"Xoá thưởng dự án - Năm: {model.NamThuong}"
                });

                // Xóa bản ghi
                DataProvider.Ins.DB.TbThuongDuAns.Remove(model);
                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Xoá thưởng dự án thành công.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Làm mới danh sách sau khi xóa
                _ = RefreshTable();
            }
            catch (DbUpdateException dbUpdateEx)
            {
                string errorMessage;

                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 547: // Vi phạm ràng buộc ngoại
                            errorMessage = "Không thể xóa vì có dữ liệu liên quan. Vui lòng kiểm tra lại.";
                            break;
                        default:
                            errorMessage = "Lỗi không xác định: " + sqlEx.Message;
                            break;
                    }

                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi không xác định trong quá trình xóa dữ liệu.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                logEx(ex);
            }
        }
        #endregion

        #region 🔄 Làm mới dữ liệu (RefreshTable)
        private CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Hủy bỏ tác vụ làm mới trước đó (nếu đang chạy)
        /// </summary>
        private void CancelPreviousTask()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Semaphore để đảm bảo chỉ 1 truy vấn DB chạy tại 1 thời điểm
        /// </summary>
        private static readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Làm mới dữ liệu bảng thưởng dự án.
        /// - Có debounce (delay 300ms) khi gõ tìm kiếm
        /// - Có cơ chế hủy task cũ nếu người dùng thay đổi nhanh
        /// - Có lock để tránh truy vấn song song gây lỗi “DbContext is already in use”
        /// </summary>
        private async Task RefreshTable()
        {
            CancelPreviousTask(); // Hủy truy vấn cũ nếu đang chạy

            try
            {
                // ⏳ Chờ 300ms để tránh reload quá nhanh khi người dùng gõ liên tục
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                await _dbLock.WaitAsync(); // 🔒 Chặn truy cập DB song song

                try
                {
                    // 📦 Truy vấn danh sách thưởng dự án
                    var query = DataProvider.Ins.DB.TbThuongDuAns
                                .AsNoTracking()
                                .AsQueryable();

                    // 👉 Có thể thêm điều kiện tìm kiếm nếu SearchFill có giá trị
                    if (!string.IsNullOrWhiteSpace(SearchFill))
                    {
                        query = query.Where(x =>
                            x.NamThuong.ToString().Contains(SearchFill));
                    }

                    // 📊 Lấy các bảng liên quan (dùng AsNoTracking để tối ưu)
                    var queryDaiDoan = await DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();

                    var queryDuAnChiTiet = await DataProvider.Ins.DB.TbThuongDuAnChiTiets
                        .AsNoTracking()
                        .ToListAsync();

                    // 🧹 Xóa danh sách cũ và gán dữ liệu mới
                    if (List == null)
                        List = new ObservableCollection<TbThuongDuAn>();
                    List.Clear();

                    foreach (var item in query)
                    {
                        // Ghép bảng TbThuongDaiDoanDuAn theo Id
                        var daiDoanList = queryDaiDoan
                            .Where(d => d.IdThuongDuAn == item.Id)
                            .ToList();

                        foreach (var t in daiDoanList)
                            item.TbThuongDaiDoanDuAns.Add(t);

                        // Ghép bảng chi tiết và tính toán "ChuaThanhToan"
                        var chiTietList = queryDuAnChiTiet
                            .Where(d => d.IdThuongDuAn == item.Id)
                            .ToList();

                        foreach (var t in chiTietList)
                        {
                            t.ChuaThanhToan = (t.DoanhThuQuyetToan ?? 0) - (t.DaThanhToan ?? 0);
                            item.TbThuongDuAnChiTiets.Add(t);
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
                // Bỏ qua lỗi “DbContext is already executing” nếu có
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua nếu người dùng hủy giữa chừng (do gõ nhanh / chuyển tab)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void DeleteDetail(TbThuongDuAnChiTiet item)
        {
            if (item == null) return;

            // Xóa khỏi ObservableCollection (UI tự cập nhật)
            TbThuongDuAn.TbThuongDuAnChiTiets.Remove(item);

            // Xóa khỏi database
            DataProvider.Ins.DB.TbThuongDuAnChiTiets.Remove(item);
            DataProvider.Ins.DB.SaveChanges();
        }

        private void CloseWindow(Window? window)
        {
            if (window != null)
            {
                List = JsonConvert.DeserializeObject<ObservableCollection<TbThuongDuAn>>(
                        JsonConvert.SerializeObject(_backupList)
);
                window.Close();
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

    }
}
