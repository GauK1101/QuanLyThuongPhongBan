using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using QuanLyThuongPhongBan.Helpers.Setting;
using QuanLyThuongPhongBan.Models.App.Settings;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Models.Settings;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Utilities;
using QuanLyThuongPhongBan.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuanLyThuongPhongBan.ViewModels
{
    public partial class ProjectBonusDetailViewModel : DataListViewModelBase
    {
        private readonly IProjectBonusDetailService _projectBonusDetailService;
        private readonly IProjectSummaryReportService _projectSummaryReportService;
        private readonly ProjectBonusCalculateOptions _settings;


        private readonly SemaphoreSlim _loadSemaphore = new SemaphoreSlim(1, 1);

        #region Properties

        [ObservableProperty]
        private ObservableCollection<ProjectBonusDetail> _projectBonusDetails;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        [NotifyCanExecuteChangedFor(nameof(CreateReportCommand))]
        private ObservableCollection<ProjectBonusDetail> _selectedProjectBonusDetails = new ObservableCollection<ProjectBonusDetail>();

        [ObservableProperty] private bool _isDetailTVGP = true;
        [ObservableProperty] private bool _isDetailHSDA = true;
        [ObservableProperty] private bool _isDetailPO = true;
        [ObservableProperty] private bool _isDetailKTK = true;
        [ObservableProperty] private bool _isDetailTTDVKT = true;
        [ObservableProperty] private bool _isUninvoicedRevenue = true;
        [ObservableProperty] private bool _isUnpaidAmount = true;
        [ObservableProperty] private bool _isAutoCalculateOnRateChange = true;

        #endregion

        public ProjectBonusDetailViewModel(IProjectBonusDetailService projectBonusDetailService, IProjectSummaryReportService projectSummaryReportService)
        {
            _projectBonusDetailService = projectBonusDetailService;
            _projectSummaryReportService = projectSummaryReportService;

            _settings = ProjectBonusCalculateOptionsHelper.LoadCalculateOptions();
            IsDetailTVGP = _settings.CalculateDetailTVGP;
            IsDetailHSDA = _settings.CalculateDetailHSDA;
            IsDetailPO = _settings.CalculateDetailPO;
            IsDetailKTK = _settings.CalculateDetailKTK;
            IsDetailTTDVKT = _settings.CalculateDetailTTDVKT;
            IsUninvoicedRevenue = _settings.CalculateUninvoicedRevenue;
            IsUnpaidAmount = _settings.CalculateUnpaidAmount;
            IsAutoCalculateOnRateChange = _settings.AutoCalculateOnRateChange;

            Task.Run(LoadDataAsync);
        }

        private async Task LoadDataAsync()
        {
            if (!await _loadSemaphore.WaitAsync(TimeSpan.Zero))
                return;

            try
            {
                IsLoading = Visibility.Visible;

                var result = await _projectBonusDetailService.GetPagedAsync(PageIndex, PageSize, FromDate, ToDate);

                MaxPageCount = result.MaxPageCount;
                FilteredRowCount = result.FilteredCount;
                TotalRowCount = result.TotalCount;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (ProjectBonusDetails is null)
                        ProjectBonusDetails = new ObservableCollection<ProjectBonusDetail>(result.Data);
                    else
                    {
                        ProjectBonusDetails.Clear();
                        foreach (var item in result.Data)
                        {
                            ProjectBonusDetails.Add(item);
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "LoadDataAsync ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
                UpdateDisplayInfo(TotalRowCount, FilteredRowCount);
                _loadSemaphore.Release();
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            ResetViewState();
            await LoadDataAsync();
            Growl.Success("Dữ liệu đã được tải thành công.");
        }

        [RelayCommand(CanExecute = nameof(CanSelectProject))]
        private async Task CreateReport()
        {
            try
            {
                if (SelectedProjectBonusDetails != null)
                {
                    if (SelectedProjectBonusDetails.Count <= 0)
                    {
                        Growl.Warning("Vui lòng chọn dự án để xóa.");
                        return;
                    }

                    var isSummaryReportService = await _projectSummaryReportService.CreateAsync(SelectedProjectBonusDetails);

                    if (isSummaryReportService)
                    {
                        Growl.Success("Tạo báo cáo thành công.");
                    }
                    else
                        Growl.Error("Tạo báo cáo thất bại.");
                }
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Addvovan ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        [RelayCommand]
        private async Task PageOn(FunctionEventArgs<int> e)
        {
            PageIndex = e.Info;
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task Add()
        {
            try
            {
                var model = await _projectBonusDetailService.CreateAsync();
                await LoadDataAsync();
                Growl.Success("Đã thêm dự án mới thành công.");
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Add ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        [RelayCommand]
        private async Task Uppdate(ProjectBonusDetail model)
        {
            try
            {
                var isUpdate = await _projectBonusDetailService.UpdateAsync(model.Id, model, _settings);

                if (isUpdate)
                    Growl.Success("Sửa thành công.");

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Uppdate ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        // Command để xóa dự án
        [RelayCommand(CanExecute = nameof(CanSelectProject))]
        private async Task Delete()
        {
            try
            {
                if (SelectedProjectBonusDetails != null)
                {
                    if (SelectedProjectBonusDetails.Count <= 0)
                    {
                        Growl.Warning("Vui lòng chọn dự án để xóa.");
                        return;
                    }

                    if (HandyControl.Controls.MessageBox.Show("Bạn có chắc chắn muốn xóa dự án đã chọn không?", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;

                    foreach (var item in SelectedProjectBonusDetails.ToList())
                    {
                        if (await _projectBonusDetailService.DeleteAsync(item.Id))
                        {
                            ProjectBonusDetails.Remove(item);
                            Growl.Success("Đã xóa dự án thành công.");
                        }
                        else
                            Growl.Error("Xóa dự án thất bại.");
                    }

                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Delete ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        private bool CanSelectProject()
        {
            return SelectedProjectBonusDetails.Count > 0;
        }

        [RelayCommand]
        private async Task FilterAsync()
        {
            try
            {
                // Validation từ service
                var validation = _projectBonusDetailService.ValidateDateRange(FromDate, ToDate);
                if (!validation.isValid)
                {
                    Growl.Warning(validation.errorMessage);
                    return;
                }

                // ✅ GỌI SERVICE để filter - business logic nằm trong service
                await LoadDataAsync();

                Growl.Success($"Đã lọc được {FilteredRowCount} kết quả");
            }
            catch (Exception ex)
            {
                Growl.Error($"Lỗi khi lọc dữ liệu: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task PasteToSelectedRow()
        {
            try
            {
                var clipboardData = Clipboard.GetText();
                if (string.IsNullOrEmpty(clipboardData))
                {
                    Growl.Warning("Không có dữ liệu trong clipboard");
                    return;
                }

                // Parse dữ liệu từ Excel (dùng Helper)
                var excelRows = ProjectBonusExcelPasteUtilities.ParseExcelData(clipboardData);
                if (!excelRows.Any())
                {
                    Growl.Warning("Không thể phân tích dữ liệu từ Excel");
                    return;
                }

                // Lấy IDs của các item được chọn
                var selectedIds = SelectedProjectBonusDetails?.Select(x => x.Id).ToList() ?? new List<int>();

                string mode = selectedIds.Any() ? "sửa" : "thêm mới";

                // Gọi service để xử lý database
                var updatedEntities = await _projectBonusDetailService.PasteExcelDataAsync(excelRows, selectedIds);

                // Load lại dữ liệu để cập nhật UI
                await LoadDataAsync();

                Growl.Success($"Đã {mode} {updatedEntities.Count} dòng thành công");
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "PasteToSelectedRow ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        partial void OnIsAutoCalculateOnRateChangeChanged(bool value) => SaveSettings();
        partial void OnIsDetailTVGPChanged(bool value) => SaveSettings();
        partial void OnIsDetailHSDAChanged(bool value) => SaveSettings();
        partial void OnIsDetailPOChanged(bool value) => SaveSettings();
        partial void OnIsDetailKTKChanged(bool value) => SaveSettings();
        partial void OnIsDetailTTDVKTChanged(bool value) => SaveSettings();
        partial void OnIsUninvoicedRevenueChanged(bool value) => SaveSettings();
        partial void OnIsUnpaidAmountChanged(bool value) => SaveSettings();

        private void SaveSettings()
        {
            var setting = new ProjectBonusCalculateOptions
            {
                AutoCalculateOnRateChange = IsAutoCalculateOnRateChange,
                CalculateDetailTVGP = IsDetailTVGP,
                CalculateDetailHSDA = IsDetailHSDA,
                CalculateDetailPO = IsDetailPO,
                CalculateDetailKTK = IsDetailKTK,
                CalculateDetailTTDVKT = IsDetailTTDVKT,
                CalculateUninvoicedRevenue = IsUninvoicedRevenue,
                CalculateUnpaidAmount = IsUnpaidAmount
            };

            ProjectBonusCalculateOptionsHelper.SaveCalculateOptions(setting);
        }
    }
}
