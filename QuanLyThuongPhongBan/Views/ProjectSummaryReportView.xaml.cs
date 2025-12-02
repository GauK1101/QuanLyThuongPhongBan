using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for ProjectSummaryReportView.xaml
    /// </summary>
    public partial class ProjectSummaryReportView : UserControl
    {
        public ProjectSummaryReportView()
        {
            InitializeComponent();

            DataContext = App.GetService<ProjectSummaryReportViewModel>();
        }
    }
}
