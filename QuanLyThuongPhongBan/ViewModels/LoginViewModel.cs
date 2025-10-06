using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.ViewForGauK.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private bool _isLogin = Properties.Settings.Default.RememberLogin;
        public bool IsLogin { get => _isLogin; set { _isLogin = value; OnPropertyChanged(); } }

        private string? _username = Properties.Settings.Default.User == string.Empty ? string.Empty : Properties.Settings.Default.User.Split('|')[0];
        public string? Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string? _password;
        public string? Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        public bool IsFirstLaunch { get; private set; } = true;

        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand LoginCheckCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<Window>(p => true, Login);
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            LoginCheckCommand = new RelayCommand<CheckBox>((p) => { return true; }, (p) => { IsLogin = p.IsChecked == true ? true : false; });

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            // Tạo một instance của UpdateViewModel
            var update = new Updater();

            // Kiểm tra cập nhật trong nền
            await update.CheckForUpdates();
        }

        private void Login(Window p)
        {
            if (p == null) return;
            var accCount = DataProvider.Ins.DB.TbTaiKhoans.Where(x => x.TenDangNhap == Username && x.MatKhau == Password).Count();


            if ((IsFirstLaunch && IsLogin) || accCount > 0)
            {
                var user = DataProvider.Ins.DB.TbTaiKhoans.FirstOrDefault(x => x.TenDangNhap == Username);
                Properties.Settings.Default.User = user == null ? string.Empty : user.TenDangNhap + "|" + user.HoTen + "|" + user.IdPhongBan;

                IsFirstLaunch = false;
                Password = string.Empty;
                p.Close();
            }
            else if (!IsFirstLaunch)
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác, vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Properties.Settings.Default.RememberLogin = IsLogin;
            Properties.Settings.Default.Save();
        }
    }
}
