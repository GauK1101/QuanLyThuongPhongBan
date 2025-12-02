using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using QuanLyThuongPhongBan.Models.App;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace QuanLyThuongPhongBan.ViewModels
{
    public partial class ProjectSummaryReportViewModel : ObservableObject
    {
        private readonly IProjectSummaryReportService _projectSummaryReportService;

        // Properties để bind từ XAML
        [ObservableProperty]
        private ObservableCollection<ProjectBonus> _projectBonuses;

        [ObservableProperty]
        private ObservableCollection<ProjectTeamBonus> _projectTeamBonuses;

        [ObservableProperty]
        private ObservableCollection<ProjectBonusDetail> _projectBonusDetails;

        [ObservableProperty]
        private ObservableCollection<Report> _reports;

        [ObservableProperty]
        private Report _selectedReport;

        public ProjectSummaryReportViewModel(IProjectSummaryReportService projectSummaryReportService)
        {
            // Khởi tạo collections
            _projectSummaryReportService = projectSummaryReportService;

            Task.Run(LoadDataAsync);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                Reports = new ObservableCollection<Report>(await _projectSummaryReportService.GetAllReportAsync());

            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "LoadDataAsync ProjectSummaryReportViewModel");
                Growl.Error(userMsg);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadDataAsync();
            Growl.Success("Dữ liệu đã được tải thành công.");
        }

        [RelayCommand]
        private async Task Delete(Report report)  // ✅ Đúng type Report
        {
            try
            {
                if (HandyControl.Controls.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa báo cáo '{report.NameReport}' không?",
                    "XÓA BÁO CÁO",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                if (await _projectSummaryReportService.DeleteAsync(report.Id))
                {
                    Growl.Success($"Đã xóa báo cáo '{report.NameReport}' thành công.");

                    // Refresh danh sách
                    await LoadDataAsync();
                }
                else
                {
                    Growl.Error("Xóa báo cáo thất bại.");
                }
            }
            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "Delete Report");
                Growl.Error(userMsg);
            }
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                base.OnPropertyChanged(e);

                if (e.PropertyName == nameof(SelectedReport))
                {
                    if (SelectedReport != null)
                    {
                        var result = await _projectSummaryReportService.GetByIdAsync(SelectedReport.Id);

                        ProjectBonuses = new ObservableCollection<ProjectBonus>(result.projectBonuses);
                        ProjectTeamBonuses = new ObservableCollection<ProjectTeamBonus>(result.projectTeamBonuses);
                        ProjectBonusDetails = new ObservableCollection<ProjectBonusDetail>(result.projectBonusDetails);
                    }
                }
            }

            catch (Exception ex)
            {
                var (userMsg, devMsg) = ErrorHelper.HandleError(ex, null, "LoadDataAsync ProjectSummaryReportViewModel");
                Growl.Error(userMsg);
            }
        }
    }
}
