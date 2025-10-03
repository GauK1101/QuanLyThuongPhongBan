using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;

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

                for (int i = 3; i <= 5; i++)
                {
                    TbThuongDaiDoanSmb detail = new TbThuongDaiDoanSmb
                    {
                        IdPhongBan = i,
                        IdPhongBanNavigation = DataProvider.Ins.DB.TbPhongBans.FirstOrDefault(x => x.Id == i)
                    };
                    model.Details.Add(detail);
                }
            }

            TbThuongSmb = model;

            aadEditSMBRewardWindow = new AddEditSMBRewardWindow();
            aadEditSMBRewardWindow.ShowDialog();

            _ = RefreshTable();
        }

        private void CalculatorData(TbThuongSmb model)
        {
            if (TbThuongSmb == null) return;

            foreach (var detail in TbThuongSmb.Details)
            {
                //detail.GiaTri = (TbThuongSmb.QuyetToan * detail.TiLeThuong) / 100;
                //detail.GiaTriDieuChinhDot1 = (TbThuongSmb.GiaTriHopDong * detail.TiLeDieuChinhDot1) / 100;
                //detail.GiaTriDieuChinhDot2 = (TbThuongSmb.GiaTriHopDong * detail.TiLeDieuChinhDot2) / 100;
                //detail.ThuHoiCongNo = detail.GiaTri - detail.GiaTriDieuChinhDot1 - detail.GiaTriDieuChinhDot2;
                //detail.NghiemThu = detail.GiaTriDieuChinhDot1 + detail.GiaTriDieuChinhDot2 + detail.ThuHoiCongNo;
            }
        }

        private void AddOrEditRow()
        {
            if (TbThuongSmb == null) return;

            try
            {
                if (TbThuongSmb.Id == 0)
                {
                    DataProvider.Ins.DB.TbThuongSmbs.Add(TbThuongSmb);

                    DataProvider.Ins.DB.SaveChanges();
                    if (MessageBox.Show("Tạo mới thành công!\nBạn thoát khỏi màn hình chinh sửa không?", "Thành công", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var currentWindow = Application.Current.Windows
                                             .OfType<Window>()
                                             .SingleOrDefault(w => w.IsActive);

                        currentWindow?.Close();
                    }
                }
                else
                {
                    var exiting = DataProvider.Ins.DB.TbThuongSmbs.FirstOrDefault(x => x.Id == TbThuongSmb.Id);

                    if (exiting == null)
                        return;

                    DataProvider.Ins.DB.Entry(exiting).State = EntityState.Detached;
                    DataProvider.Ins.DB.Entry(TbThuongSmb).State = EntityState.Modified;

                    foreach (var detail in TbThuongSmb.Details.ToList())
                    {
                        var exitingDetail = DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                            .FirstOrDefault(x => x.Id == detail.Id && x.IdThuongSmb == TbThuongSmb.Id);
                        if (exitingDetail != null)
                        {
                            DataProvider.Ins.DB.Entry(exitingDetail).State = EntityState.Detached;
                            DataProvider.Ins.DB.Entry(detail).State = EntityState.Modified;
                        }
                        else
                        {
                            detail.IdThuongSmb = TbThuongSmb.Id;
                            DataProvider.Ins.DB.TbThuongDaiDoanSmbs.Add(detail);
                        }
                    }

                    TbThuongSmb.Details = new ObservableCollection<TbThuongDaiDoanSmb>(
                        DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                            .Where(d => d.IdThuongSmb == TbThuongSmb.Id)
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

        private void DeleteRow(TbThuongSmb model)
        {
            try
            {
                if (MessageBox.Show("Bạn có thật sự muốn xoá không ?", "Xoá", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;

                var data = DataProvider.Ins.DB.TbThuongDaiDoanSmbs.FirstOrDefault(x => x.IdThuongSmb == model.Id);
                if (data != null)
                    DataProvider.Ins.DB.TbThuongDaiDoanSmbs.Remove(data);

                DataProvider.Ins.DB.TbThuongSmbs.Remove(model);

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

                await _dbLock.WaitAsync(); // 🔒 Chặn các truy vấn song song
                try
                {
                    // Truy vấn dữ liệu chính
                    var query = await DataProvider.Ins.DB.TbThuongSmbs
                        .AsNoTracking()
                        .ToListAsync();

                    // Truy vấn dữ liệu chi tiết
                    var queryDetails = await DataProvider.Ins.DB.TbThuongDaiDoanSmbs
                        .AsNoTracking()
                        .Include(d => d.IdPhongBanNavigation)
                        .ToListAsync();


                    //// Áp dụng tìm kiếm trực tiếp trên cơ sở dữ liệu
                    //query = SearchViewModel.Search(query, SearchFill ?? string.Empty); // Cập nhật SearchViewModel để trả về IQueryable

                    //// Thực hiện phân trang
                    //var data = await query
                    //    .Skip((currentPage - 1) * pageSize)
                    //    .Take(pageSize)
                    //    .ToListAsync();

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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
