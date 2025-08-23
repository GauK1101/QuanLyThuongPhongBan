using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace QuanLyThuongPhongBan.ViewForGauK.View
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    /// 
    public partial class UpdateWindow : Window
    {
        private List<string> _imageUrls;
        private List<string> _images = new List<string>();

        public UpdateWindow()
        {
            InitializeComponent();
            InitializeImageUrls();
            SetRandomImage();
        }

        public void UpdateStatus(string status)
        {
            txtStatus.Text = status;
        }

        public void UpdateProgress(int progress)
        {
            progressBar.Value = progress;
        }

        #region Random Image
        private void InitializeImageUrls()
        {
            _imageUrls = new List<string>
            {
                "https://img.freepik.com/free-photo/japan-background-digital-art_23-2151546131.jpg",
                "https://img.freepik.com/free-photo/anime-style-galaxy-background_23-2151133930.jpg",
                "https://wallpapers.com/images/hd/anime-cherry-blossom-background-99q9j0o6ap26ngdx.jpg",
                "https://w0.peakpx.com/wallpaper/409/61/HD-wallpaper-anime-scenery-train-station-girl-school-uniform-sunset-anime.jpg",
                "https://w0.peakpx.com/wallpaper/925/681/HD-wallpaper-anime-landscape-falling-star-train-station-trip-scenery-anime-girl-anime.jpg",
            };
        }

        private void SetRandomImage()
        {
            var random = new Random();
            string randomUrl = _imageUrls[random.Next(_imageUrls.Count)];
            imageBrush.ImageSource = new BitmapImage(new Uri(randomUrl));
        }

        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            SetRandomImage();
        }
        #endregion

        #region Control Bar
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
