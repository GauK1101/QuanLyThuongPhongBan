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
        #endregion

        public MainViewModel()
        {
            ShowProjectRewardView();

            ShowProjectRewardViewCommand = new RelayCommand<object>(_ => true, _ => ShowProjectRewardView());
            ShowSMBRewardViewCommand = new RelayCommand<object>(_ => true, _ => ShowSMBRewardView());
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
