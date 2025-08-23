using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using QuanLyThuongPhongBan.ViewForGauK.View;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        #region Properties
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private string _Caption;

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

        private Visibility _VisibilityAdmin = Visibility.Collapsed;
        public Visibility VisibilityAdmin
        {
            get => _VisibilityAdmin;
            set
            {
                _VisibilityAdmin = value;
                OnPropertyChanged();
            }
        }

        private Visibility _VisibilityAccount = Visibility.Visible;
        public Visibility VisibilityAccount
        {
            get => _VisibilityAccount;
            set
            {
                _VisibilityAccount = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Notifications { get; set; }
        #endregion

        #region Command
        public ICommand LogoutCommand { get; }

        public ICommand AccountCommand { get; }

        public ICommand AdminCommand { get; }

        public ICommand LoadedWindowCommand { get; }

        public ICommand ShowHomeViewCommand { get; }

        public ICommand ShowOrderAndProductViewCommand { get; }

        public ICommand ShowProductInventoryViewCommand { get; }

        public ICommand ShowManufacturerViewCommand { get; }

        public ICommand ShowCustomerViewModelCommand { get; }
        #endregion

#pragma warning disable CS8618
        private DispatcherTimer updateTimer;

        public MainViewModel()
        {
        }

        //private void ShowCustomerViewModel()
        //{
        //    CurrentView = new CustomerView();
        //    Caption = "Khách hàng";
        //    Icon = PackIconKind.Account;
        //}
    }
}
