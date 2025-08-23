using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using QuanLyThuongPhongBan.ViewForGauK.View;
using QuanLyThuongPhongBan.ViewForGauK.ViewModels;

namespace QuanLyThuongPhongBan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer updateTimer;

        public MainWindow()
        {
            InitializeComponent();
            Update();

            UpdateWindow up = new UpdateWindow();
            this.Hide();
            up.ShowDialog();
            this.Show();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMinutes(5); // Kiểm tra mỗi 5 phút
            updateTimer.Tick += (s, e) => Update();
            updateTimer.Start();
        }

        private async void Update()
        {
            Updater updater = new Updater();
            await updater.CheckForUpdates();
        }

        #region Control Bar
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Btn_Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Title_Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
        #endregion
    }
}