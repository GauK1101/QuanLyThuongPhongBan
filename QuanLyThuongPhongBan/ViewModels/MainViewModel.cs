using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Views;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using static QuanLyThuongPhongBan.CLass.SearchFilter;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        #region Properties
        private ObservableCollection<TbNhatKy>? _list;
        public ObservableCollection<TbNhatKy>? List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

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
        private Visibility _visibilityAdmin = Visibility.Hidden;
        public Visibility VisibilityAdmin
        {
            get => _visibilityAdmin;
            set
            {
                _visibilityAdmin = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibilityTVGP = Visibility.Hidden;
        public Visibility VisibilityTVGP
        {
            get => _visibilityTVGP;
            set
            {
                _visibilityTVGP = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibilityHSDA = Visibility.Hidden;
        public Visibility VisibilityHSDA
        {
            get => _visibilityHSDA;
            set
            {
                _visibilityHSDA = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibilityNP = Visibility.Hidden;
        public Visibility VisibilityNP
        {
            get => _visibilityNP;
            set
            {
                _visibilityNP = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibilityKTK = Visibility.Hidden;
        public Visibility VisibilityKTK
        {
            get => _visibilityKTK;
            set
            {
                _visibilityKTK = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibilityKT = Visibility.Hidden;
        public Visibility VisibilityKT
        {
            get => _visibilityKT;
            set
            {
                _visibilityKT = value;
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
            LogoutCommand = new RelayCommand<Window>(_ => true, window =>
            {
                // Lưu trạng thái RememberLogin
                Properties.Settings.Default.User = string.Empty;
                Properties.Settings.Default.RememberLogin = false;
                Properties.Settings.Default.Save();

                // Gọi logic logout (Load là method của bác)
                Load(window);
            });
        }

        private async void Load(Window p)
        {
            try
            {
                p.Hide();

                LoginWindow lg = new LoginWindow();
                lg.ShowDialog();

                if (p == null)
                    return;
                p.Show();


                if (Properties.Settings.Default.User == string.Empty)
                    return;

                if (Properties.Settings.Default.User.Split('|')[2] == "0")
                {
                    VisibilityAdmin = Visibility.Visible;
                    VisibilityTVGP = Visibility.Visible;
                    VisibilityHSDA = Visibility.Visible;
                    VisibilityNP = Visibility.Visible;
                    VisibilityKTK = Visibility.Visible;
                    VisibilityKT = Visibility.Visible;
                }
                else if (Properties.Settings.Default.User.Split('|')[2] == "1")
                {
                    VisibleAuthem();
                    VisibilityTVGP = Visibility.Visible;
                }
                else if (Properties.Settings.Default.User.Split('|')[2] == "2")
                {
                    VisibleAuthem();
                    VisibilityHSDA = Visibility.Visible;
                }
                else if (Properties.Settings.Default.User.Split('|')[2] == "3")
                {
                    VisibleAuthem();
                    VisibilityNP = Visibility.Visible;
                }
                else if (Properties.Settings.Default.User.Split('|')[2] == "4")
                {
                    VisibleAuthem();
                    VisibilityKTK = Visibility.Visible;
                }
                else if (Properties.Settings.Default.User.Split('|')[2] == "5")
                {
                    VisibleAuthem();
                    VisibilityKT = Visibility.Visible;
                }

                try
                {
                    // Truy vấn cơ sở dữ liệu với phân trang và tìm kiếm trực tiếp
                    var query = DataProvider.Ins.DB.TbNhatKies
                        .AsNoTracking() // chỉ gọi 1 lần
                        .ToList();       // bắt buộc thực thi query

                    if (List == null)
                        List = new ObservableCollection<TbNhatKy>();
                    else
                        List.Clear();

                    foreach (var item in query)
                    {
                        List.Add(item);
                    }
                }
                catch (Exception)
                {
                    //Không báo lỗi khi tắt màn login đột ngột
                }

            }
            catch (InvalidOperationException)
            {
                //Không báo lỗi khi tắt màn login đột ngột
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VisibleAuthem()
        {
            VisibilityAdmin = Visibility.Hidden;
            VisibilityTVGP = Visibility.Hidden;
            VisibilityHSDA = Visibility.Hidden;
            VisibilityNP = Visibility.Hidden;
            VisibilityKTK = Visibility.Hidden;
            VisibilityKT = Visibility.Hidden;
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
