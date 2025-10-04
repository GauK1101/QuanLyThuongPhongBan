using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i)
                    };
                    model.Details.Add(detail);
                }
            }

            model.NamThuong = DateTime.Now.Date.Year.ToString();
            TbThuongDuAn = model;
            
            addEditProjectRewardWindow = new AddEditProjectRewardWindow();
            addEditProjectRewardWindow.ShowDialog();
        }

        private void CalculatorData(TbThuongDuAn model)
        {
            if (TbThuongDuAn == null) return;

            foreach (var detail in TbThuongDuAn.Details)
            {
                detail.GiaTriTongGoi = (TbThuongDuAn.QuyetToan * detail.TiLeTongGoi) / 100;
                detail.GiaTriDieuChinhDot1 = (TbThuongDuAn.GiaTriHopDong * detail.TiLeDieuChinhDot1) / 100;
                detail.GiaTriDieuChinhDot2 = (TbThuongDuAn.GiaTriHopDong * detail.TiLeDieuChinhDot2) / 100;
                detail.ThuHoiCongNo = detail.GiaTriTongGoi - detail.GiaTriDieuChinhDot1 - detail.GiaTriDieuChinhDot2;
                detail.NghiemThu = detail.GiaTriDieuChinhDot1 + detail.GiaTriDieuChinhDot2 + detail.ThuHoiCongNo;
            
                TbThuongDuAn.TongTiLeThuongDuAn = TbThuongDuAn.Details.Sum(d => d.TiLeTongGoi);
                TbThuongDuAn.TongGiaTriThuongDuAn = TbThuongDuAn.Details.Sum(d => d.GiaTriTongGoi);
            }
        }

        private void AddOrEditRow()
        {
            if (TbThuongDuAn == null) return;

            try
            {
                if (TbThuongDuAn.Id == 0)
                {
                    DataProvider.Ins.DB.TbThuongDuAns.Add(TbThuongDuAn);

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Tạo mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var exiting = DataProvider.Ins.DB.TbThuongDuAns.FirstOrDefault(x => x.Id == TbThuongDuAn.Id);

                    if (exiting == null)
                        return;

                    //DataProvider.Ins.DB.Entry(exiting).State = EntityState.Detached;
                    //DataProvider.Ins.DB.Entry(TbThuongDuAn).State = EntityState.Modified;
                    DataProvider.Ins.DB.Entry(exiting).CurrentValues.SetValues(TbThuongDuAn);

                    foreach (var detail in TbThuongDuAn.Details.ToList())
                    {
                        var exitingDetail = DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                            .FirstOrDefault(x => x.Id == detail.Id && x.IdThuongDuAn == TbThuongDuAn.Id);
                        if (exitingDetail != null)
                        {
                            //DataProvider.Ins.DB.Entry(exitingDetail).State = EntityState.Detached;
                            //DataProvider.Ins.DB.Entry(detail).State = EntityState.Modified;
                            DataProvider.Ins.DB.Entry(exitingDetail).CurrentValues.SetValues(detail);
                        }
                        else
                        {
                            detail.IdThuongDuAn = TbThuongDuAn.Id;
                            DataProvider.Ins.DB.TbThuongDaiDoanDuAns.Add(detail);
                        }
                    }

                    TbThuongDuAn.Details = new ObservableCollection<TbThuongDaiDoanDuAn>(
                        DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                            .Where(d => d.IdThuongDuAn == TbThuongDuAn.Id)
                            .Include(d => d.IdPhongBanNavigation)
                            .ToList()
                    );

                    DataProvider.Ins.DB.SaveChanges();
                    if (MessageBox.Show("Sửa thành công!\nBạn thoát khỏi màn hình chinh sửa không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
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
                MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRow(TbThuongDuAn model)
        {
            try
            {
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?", "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;

                var data = DataProvider.Ins.DB.TbThuongDaiDoanDuAns.FirstOrDefault(x => x.IdThuongDuAn == model.Id);
                if (data != null)
                    DataProvider.Ins.DB.TbThuongDaiDoanDuAns.Remove(data);

                DataProvider.Ins.DB.TbThuongDuAns.Remove(model);

                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Xoá đơn hàng và sản phẩm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    query = SearchViewModel.Search(query, SearchFill ?? string.Empty); // Cập nhật SearchViewModel để trả về IQueryable


                    var queryDetails = await DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();

                    if (List == null)
                        List = new ObservableCollection<TbThuongDuAn>();
                    List.Clear();

                    foreach (var item in query)
                    {
                        var details = queryDetails
                                        .Where(d => d.IdThuongDuAn == item.Id)
                                        .Select(d => new TbThuongDaiDoanDuAn
                                        {
                                            Id = d.Id,
                                            IdPhongBan = d.IdPhongBan,
                                            IdThuongDuAn = d.IdThuongDuAn,
                                            TiLeTongGoi = d.TiLeTongGoi,
                                            GiaTriTongGoi = d.GiaTriTongGoi,
                                            GiaTriDieuChinhDot1 = d.GiaTriDieuChinhDot1,
                                            TiLeDieuChinhDot1 = d.TiLeDieuChinhDot1,
                                            GiaTriDieuChinhDot2 = d.GiaTriDieuChinhDot2,
                                            TiLeDieuChinhDot2 = d.TiLeDieuChinhDot2,
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
            catch (Exception ex)
            {
                //MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
