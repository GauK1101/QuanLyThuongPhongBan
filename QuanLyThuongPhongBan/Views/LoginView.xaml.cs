using QuanLyThuongPhongBan.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private bool _isPasswordVisible = false;

        public LoginView()
        {
            InitializeComponent();
            DataContext = App.GetService<LoginViewModel>();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PwdBox.Password;
            PlaceholderText.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        private void PwdBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderText.Visibility = Visibility.Collapsed;
        }

        private void PwdBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PwdBox.Password))
                PlaceholderText.Visibility = Visibility.Visible;
        }
    }
}
