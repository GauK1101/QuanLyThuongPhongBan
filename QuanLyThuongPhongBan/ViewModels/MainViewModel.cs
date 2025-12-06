using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Views.Login;
using System.Windows;

namespace QuanLyThuongPhongBan.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ILoginRememberService _rememberService;

        [ObservableProperty] private Visibility _smbTabVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _projectTabVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _reportTabVisibility = Visibility.Collapsed;

        public MainViewModel(ICurrentUserService currentUserService, ILoginRememberService rememberService)
        {
            _currentUser = currentUserService;
            _rememberService = rememberService;
            UpdateTabVisibility();
        }

        private void UpdateTabVisibility()
        {
            if (_currentUser.CurrentUser == null)
            {
                SmbTabVisibility = Visibility.Collapsed;
                ProjectTabVisibility = Visibility.Collapsed;
                ReportTabVisibility = Visibility.Collapsed;
                return;
            }

            var role = _currentUser.CurrentUser.Role;

            // Bác tự điều chỉnh theo yêu cầu thực tế nha
            SmbTabVisibility = role == "Admin" || role == "ketoan" ? Visibility.Visible : Visibility.Collapsed;
            ProjectTabVisibility = role == "Admin" || role == "hsda" ? Visibility.Visible : Visibility.Collapsed;
            ReportTabVisibility = role == "Admin" || role == "ketoan" || role == "hsda" ? Visibility.Visible : Visibility.Collapsed;
        }

        [RelayCommand]
        private void Logout()
        {
            // 1. Xóa user hiện tại
            _currentUser.Logout();
            _rememberService.ClearRememberedLogin();

            // 2. Mở lại cửa sổ Login
            var loginWindow = new LoginView();
            loginWindow.Show();

            // 3. Đóng MainWindow hiện tại (an toàn, không lỗi)
            Application.Current.MainWindow?.Close();
        }
    }
}
