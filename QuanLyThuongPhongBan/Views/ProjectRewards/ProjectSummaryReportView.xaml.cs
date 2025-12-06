using QuanLyThuongPhongBan.ViewModels;
using System.Windows.Controls;

namespace QuanLyThuongPhongBan.Views.ProjectRewards
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
