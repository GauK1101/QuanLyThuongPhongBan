using QuanLyThuongPhongBan.ViewModels;
using System.Windows.Controls;

namespace QuanLyThuongPhongBan.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CalculateOptionsDialog.xaml
    /// </summary>
    public partial class CalculateOptionsDialog : UserControl
    {
        public CalculateOptionsDialog()
        {
            InitializeComponent();
            DataContext = App.GetService<SmbRewardViewModel>();
        }

        private void Click_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }
    }
}
