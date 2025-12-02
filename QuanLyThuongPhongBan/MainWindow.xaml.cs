using QuanLyThuongPhongBan.ViewModels;
using QuanLyThuongPhongBan.ViewOfGauK.ViewModels;

namespace QuanLyThuongPhongBan
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            var test = new UpdaterViewModel();
            _ = test.CheckForUpdates();
        }
    }
}
