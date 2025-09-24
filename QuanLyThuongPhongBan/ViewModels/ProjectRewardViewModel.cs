using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class ProjectRewardViewModel : BaseViewModel
    {
        public ICommand? ShowAddOrEditRowCommand { get; }
        public ICommand? AddOrEditRowCommand { get; }
        public ICommand? ReloadDataCommand { get; }

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

        public ProjectRewardViewModel()
        {
            TbThuongDaiDoanDuAn = new TbThuongDaiDoanDuAn();
            TbThuongDuAn = new TbThuongDuAn();

            Load();

            ShowAddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, ShowAddOrEditRow);
            AddOrEditRowCommand = new RelayCommand<TbThuongDuAn>(_ => true, _ => AddOrEditRow());
            ReloadDataCommand = new RelayCommand<object>(_ => true, _ => Load());
        }

        private void Load()
        {
            _ = RefreshTable();
        }

        private void ShowAddOrEditRow(TbThuongDuAn model)
        {
            //if (Authorization == "Guest")
            //{
            //    MessageNotifyWindow.Show("Bạn không đủ thẩm quyền để làm việc này!", Colors.Orange);
            //    return;
            //}

            if (TbThuongDuAn == null) return;

            if (model == null){

                model = new TbThuongDuAn();

                for (int i = 1; i <= 5; i++)
                {
                    TbThuongDaiDoanDuAn detail = new TbThuongDaiDoanDuAn
                    {
                        IdPhongBan = i
                    };
                    model.Details.Add(detail);
                }
            }

            TbThuongDuAn = model;

            addEditProjectRewardWindow = new AddEditProjectRewardWindow();
            addEditProjectRewardWindow.ShowDialog();

            _ = RefreshTable();
        }

        private void AddOrEditRow()
        {
            if (TbThuongDuAn == null) return;

            string changeProperties = string.Empty;

            try
            {
                foreach (var detail in TbThuongDuAn.Details)
                {
                    DataProvider.Ins.DB.TbPhongBans.Any(x => x.Id == detail.IdPhongBan);
                    MessageBox.Show("Không có phòng ban !", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (TbThuongDuAn.Id == 0)
                {
                    DataProvider.Ins.DB.TbThuongDuAns.Add(TbThuongDuAn);

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Tạo mới thành công!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var exiting = DataProvider.Ins.DB.TbThuongDuAns.FirstOrDefault(x => x.Id == TbThuongDuAn.Id);

                    if (exiting == null)
                        return;

                    DataProvider.Ins.DB.Entry(exiting).State = EntityState.Detached;
                    DataProvider.Ins.DB.Entry(TbThuongDuAn).State = EntityState.Modified;

                    //changeProperties = CopyAndDetectChanges(TbThuongDuAn, exiting);

                    if (!string.IsNullOrEmpty(changeProperties))
                        DataProvider.Ins.DB.SaveChanges();
                    else
                    {
                        //MessageNotifyWindow.Show("Không có thay đổi nào xảy ra cả.");
                        return;
                    }
                }

                //if (!string.IsNullOrEmpty(changeProperties))
                //{
                //    DataProvider.Ins.DB.ThongBaos
                //        .Add(
                //            new ThongBao
                //            {
                //                TieuDe = "",
                //                TinNhan = $"{NameAccount} đã {changeProperties}",
                //                ThoiGianTao = DateTime.Now
                //            });
                //}

                //addorEditCustomerView?.Close();

                //DataProvider.Ins.DB.SaveChanges();

                //MessageNotifyWindow.Show($"{NameAccount} đã {changeProperties} thành công");
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
                    errorMessage = "Lỗi không xác định trong quá trình cập nhật dữ liệu.";
                }

                //MessageNotifyWindow.Show($"Thêm mới hoặc sửa không thành công:\n{errorMessage}", Colors.Red);
            }
            catch (Exception ex)
            {
                //MessageNotifyWindow.Show($"Lỗi không xác định:\n{ex.Message}", Colors.Red);
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

        private async Task RefreshTable()
        {
            CancelPreviousTask();

            try
            {
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                // Truy vấn cơ sở dữ liệu với phân trang và tìm kiếm trực tiếp
                var query = await DataProvider.Ins.DB.TbThuongDuAns
                    .AsNoTracking()
                    .AsQueryable()
                    .ToListAsync();

                var queryDetails = await DataProvider.Ins.DB.TbThuongDaiDoanDuAns
                    .AsNoTracking()
                    .Include(d => d.IdPhongBanNavigation)
                    .AsQueryable()
                    .ToListAsync();

                //// Áp dụng tìm kiếm trực tiếp trên cơ sở dữ liệu
                //query = SearchViewModel.Search(query, SearchFill ?? string.Empty); // Cập nhật SearchViewModel để trả về IQueryable

                //// Thực hiện phân trang
                //var data = await query
                //    .Skip((currentPage - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync();

                if (List == null)
                    List = new ObservableCollection<TbThuongDuAn>();
                List.Clear();

                foreach (var item in query)
                {
                    var details = queryDetails
                                    .Where(d => d.IdThuongDuAnPhongBan == item.Id)
                                    .Select(d => new TbThuongDaiDoanDuAn
                                    {
                                        Id = d.Id,
                                        IdPhongBan = d.IdPhongBan,
                                        IdThuongDuAnPhongBan = d.IdThuongDuAnPhongBan,
                                        TiLeThuong = d.TiLeThuong,
                                        GiaTri = d.GiaTri,
                                        GiaTriDieuChinhDot1 = d.GiaTriDieuChinhDot1,
                                        TiLeDieuChinhDot1 = d.TiLeDieuChinhDot1,
                                        GiaTriDieuChinhDot2 = d.GiaTriDieuChinhDot2,
                                        TiLeDieuChinhDot2 = d.TiLeDieuChinhDot2,
                                        ThuHoiCongNo = d.ThuHoiCongNo,
                                        NghiemThu = d.NghiemThu,
                                        IdPhongBanNavigation = d.IdPhongBanNavigation
                                    });

                    // Gán vào Details của item
                    foreach (var ct in details)
                    {
                        item.Details.Add(ct);
                    }

                    // Thêm item vào List chính
                    List.Add(item);
                }
            }
            catch (Exception) { }
        }
    }
}
