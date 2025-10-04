using MaterialDesignThemes.Wpf;
using QuanLyThuongPhongBan.ViewForGauK.View;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        #region Properties
        private object _currentView = null!;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private string _Caption = string.Empty;

        public string Caption
        {
            get => _Caption;
            set
            {
                _Caption = value;
                OnPropertyChanged();
            }
        }

        private PackIconKind _Icon;

        public PackIconKind Icon
        {
            get => _Icon;
            set
            {
                _Icon = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        public ICommand ShowProjectRewardViewCommand { get; }
        public ICommand ShowSMBRewardViewCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadedWindowCommand { get; }

        #endregion

        public ObservableCollection<string> Notifications { get; set; }

        public MainViewModel()
        {
            ShowProjectRewardView();

            ShowProjectRewardViewCommand = new RelayCommand<object>(_ => true, _ => ShowProjectRewardView());
            ShowSMBRewardViewCommand = new RelayCommand<object>(_ => true, _ => ShowSMBRewardView());

            LoadedWindowCommand = new RelayCommand<Window>(_ => true, Load);
            LogoutCommand = new RelayCommand<Window>(_ => true, Load);
        }

        private async void Load(Window p)
        {
            try
            {
                p.Hide();


                // Tạo một instance của UpdateViewModel
                //var updateViewModel = new UpdateViewModel();

                // Kiểm tra cập nhật trong nền
                //bool isUpdateAvailable = await updateViewModel.CheckAndDownloadUpdate();

                //if (isUpdateAvailable)
                //{
                //    // Tạo cửa sổ cập nhật và gán ViewModel
                //    UpdateWindow updateWindow = new UpdateWindow
                //    {
                //        DataContext = updateViewModel
                //    };
                //    updateWindow.Show();
                //    await updateViewModel.DownloadFile();
                //}
                //else
                LoginWindow lg = new LoginWindow();
                lg.ShowDialog();

                //var loginVM = lg.DataContext as LoginWindow;

                //if (loginVM.Authorization == "Admin")
                //    VisibilityAdmin = Visibility.Visible;
                //else
                //    VisibilityAdmin = Visibility.Collapsed;

                //if (loginVM == null || !loginVM.IsLogin)
                //    return;

                if (p == null)
                    return;
                p.Show();
            }
            catch (InvalidOperationException)
            {
                //Không báo lỗi khi tắt màn login đột ngột
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowProjectRewardView()
        {
            CurrentView = new ProjectRewardView();
            Caption = "Thưởng dự án";
            Icon = PackIconKind.Newspaper;
        }

        private void ShowSMBRewardView()
        {
            CurrentView = new SMBRewardView();
            Caption = "Thưởng SMB";
            Icon = PackIconKind.Storage;
        }
    }
}
