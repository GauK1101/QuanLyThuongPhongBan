using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Models.App.SettingsSmb;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.ViewModels.Base;
using QuanLyThuongPhongBan.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal partial class SmbRewardViewModel : DataListViewModelBase
    {
        private readonly ISmbRewardService _smbRewardService;
        private readonly SemaphoreSlim _loadSemaphore = new SemaphoreSlim(1, 1);

        #region Properties
        [ObservableProperty] private ObservableCollection<SmbBonus> _smbBonus;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private ObservableCollection<SmbBonus> _selectedSmbBonuses = new ObservableCollection<SmbBonus>();

        [ObservableProperty] private SmbTeamBonus? _selectedSmbTeamBonus;

        [ObservableProperty] private SmbBonus? _selectedSmbBonus;

        [ObservableProperty] private int _selectedIndex = -1;

        [ObservableProperty] private bool _isPhase1Value = true;
        [ObservableProperty] private bool _isTotalSmbValue = true;
        [ObservableProperty] private bool _isDebtRecovery = true;
        [ObservableProperty] private bool _isAcceptance = true;
        [ObservableProperty] private bool _isAutoCalculateOnRateChange = true;
        #endregion

        public SmbRewardViewModel(ISmbRewardService smbRewardService)
        {
            _smbRewardService = smbRewardService;

            var setting = SettingsSmbHelper.LoadCalculateOptions();
            IsPhase1Value = setting.CalculatePhase1Value;
            IsTotalSmbValue = setting.CalculateTotalSmbValue;
            IsDebtRecovery = setting.CalculateDebtRecovery;
            IsAcceptance = setting.CalculateAcceptance;
            IsAutoCalculateOnRateChange = setting.AutoCalculateOnRateChange;

            Task.Run(LoadDataAsync);
        }

        private async Task LoadDataAsync()
        {
            // ✅ Tránh concurrent requests
            if (!await _loadSemaphore.WaitAsync(TimeSpan.Zero))
                return;

            try
            {
                IsLoading = Visibility.Visible;

                // ✅ Chạy database query trong background
                var result = await Task.Run(() =>
                    _smbRewardService.GetPagedAsync(PageIndex, PageSize, SearchKeyword)
                ).ConfigureAwait(false);

                // ✅ Chỉ dùng UI thread cho data binding
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MaxPageCount = result.MaxPageCount;
                    FilteredRowCount = result.FilteredCount;
                    TotalRowCount = result.TotalCount;
                    UpdateCollection(result.Data);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua khi cancel
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "LoadDataAsync");
                Growl.Error(userMsg);
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
                UpdateDisplayInfo(TotalRowCount, FilteredRowCount);
                _loadSemaphore.Release();
            }
        }

        private void UpdateCollection(List<SmbBonus> data)
        {
            if (SmbBonus == null)
            {
                SmbBonus = new ObservableCollection<SmbBonus>(data);
            }
            else
            {
                // ✅ Tối ưu hóa: chỉ update nếu data thực sự thay đổi
                if (SmbBonus.SequenceEqual(data)) return;

                SmbBonus.Clear();
                foreach (var item in data)
                {
                    SmbBonus.Add(item);
                }
            }
        }

        [RelayCommand]
        private async Task PageOn(FunctionEventArgs<int> e)
        {
            PageIndex = e.Info;
            await LoadDataAsync(); // ✅ Load data bất đồng bộ
        }

        [RelayCommand]
        private async Task Refresh()
        {
            var currentSelectedId = SelectedSmbBonus?.Id ?? 0;

            await LoadDataAsync();

            // Sau khi load xong → khôi phục lại dòng cũ
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (currentSelectedId > 0 && SmbBonus != null)
                {
                    var itemToSelect = SmbBonus.FirstOrDefault(x => x.Id == currentSelectedId);
                    if (itemToSelect != null)
                    {
                        SelectedSmbBonus = itemToSelect; // tự động trigger ScrollIntoView
                    }
                }

                Growl.Success("Dữ liệu đã được làm mới!");
            });
        }

        [RelayCommand]
        private async Task Add()
        {
            try
            {
                await _smbRewardService.CreateAsync();
                await Refresh();
                Growl.Success("Đã thêm dự án mới thành công.");
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Add ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        [RelayCommand]
        private async Task Uppdate(SmbBonus model)
        {
            try
            {
                var isUpdate = await _smbRewardService.UpdateAsync(model.Id, model, IsAutoCalculateOnRateChange);

                if (isUpdate)
                {
                    Growl.Success("Sửa thành công.");
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Uppdate ProjectBonusDetailViewModel");
                Growl.Error(userMsg);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSelectProject))]
        private async Task Delete()
        {
            try
            {
                if (SelectedSmbBonuses != null)
                {
                    if (SelectedSmbBonuses.Count <= 0)
                    {
                        Growl.Warning("Vui lòng chọn dự án để xóa.");
                        return;
                    }

                    if (HandyControl.Controls.MessageBox.Show("Bạn có chắc chắn muốn xóa dự án đã chọn không?", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;

                    foreach (var item in SelectedSmbBonuses.ToList())
                    {
                        if (await _smbRewardService.DeleteAsync(item.Id))
                        {
                            SelectedSmbBonuses.Remove(item);
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
            return SelectedSmbBonuses.Count > 0;
        }

        [RelayCommand]
        private async Task Search()
        {
            try
            {
                await LoadDataAsync();
                Growl.Success($"Đã lọc được {FilteredRowCount} kết quả");
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Search SmbRewardViewModel");
                Growl.Error(userMsg);
            }
        }


        [RelayCommand]
        private void ShowSettings()
        {
            Dialog.Show(new CalculateOptionsDialog());
        }

        partial void OnIsPhase1ValueChanged(bool value) => SaveSettings();
        partial void OnIsTotalSmbValueChanged(bool value) => SaveSettings();
        partial void OnIsDebtRecoveryChanged(bool value) => SaveSettings();
        partial void OnIsAcceptanceChanged(bool value) => SaveSettings();
        partial void OnIsAutoCalculateOnRateChangeChanged(bool value) => SaveSettings();


        private void SaveSettings()
        {
            var setting = new CalculateOptionsSetting
            {
                CalculatePhase1Value = IsPhase1Value,
                CalculateTotalSmbValue = IsTotalSmbValue,
                CalculateDebtRecovery = IsDebtRecovery,
                CalculateAcceptance = IsAcceptance,
                AutoCalculateOnRateChange = IsAutoCalculateOnRateChange,
            };
            SettingsSmbHelper.SaveCalculateOptions(setting);
        }
    }
}
