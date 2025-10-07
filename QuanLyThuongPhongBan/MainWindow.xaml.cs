using QuanLyThuongPhongBan.ViewForGauK.ViewModels;
using System.Windows;

namespace QuanLyThuongPhongBan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowPopup_Click(object sender, RoutedEventArgs e)
        {
            NotificationPopup.IsOpen = true;
        }
    }
}