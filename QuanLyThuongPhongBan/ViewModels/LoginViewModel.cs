using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Views.Login;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QuanLyThuongPhongBan.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ILoginRememberService _rememberService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty] private string _username = "";
        [ObservableProperty] private string _password = "";
        [ObservableProperty] private bool _rememberMe;
        [ObservableProperty] private bool _role;

        public LoginViewModel(IAuthenticationService authenticationService, ILoginRememberService rememberService, ICurrentUserService currentUserService)
        {
            _rememberService = rememberService;
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;

            // Load thông tin đã nhớ khi mở form
            var remembered = _rememberService.LoadRememberedLogin();
            Username = remembered.Username ?? "";
            Password = remembered.EncryptedPassword ?? ""; // đã được giải mã
            RememberMe = remembered.RememberMe;

            // Nếu có nhớ + có mật khẩu → tự động đăng nhập luôn
            if (RememberMe && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    await Task.Delay(100);
                    await LoginCommand.ExecuteAsync(null);
                });
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //IsLoading = true;
            var user = await _authenticationService.LoginAsync(Username, Password);

            if (user != null)
            {
                // Lưu nhớ đăng nhập nếu người dùng chọn
                if (RememberMe)
                {
                    _rememberService.SaveRememberedLogin(new LoginRemember
                    {
                        Username = Username,
                        EncryptedPassword = Password,
                        RememberMe = true,
                    });
                }
                else
                {
                    _rememberService.ClearRememberedLogin();
                }

                _currentUserService.SetCurrentUser(user);

                Growl.SuccessGlobal($"Chào mừng !");

                var mainWindow = App.GetService<MainWindow>();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                var loginWindow = Application.Current.Windows.OfType<LoginView>().FirstOrDefault();
                if (loginWindow != null && loginWindow.IsLoaded)
                {
                    loginWindow.Close();
                }
                else
                {
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
            }
            else
            {
                Growl.Error("Sai tên đăng nhập hoặc mật khẩu!", "loginMsg");
            }
        }
    }
}

