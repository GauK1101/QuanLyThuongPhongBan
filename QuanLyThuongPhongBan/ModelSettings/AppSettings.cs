using System.ComponentModel;

namespace QuanLyThuongPhongBan.ModelSettings
{
    public class AppSettings : INotifyPropertyChanged
    {
        private bool _isCurrencySymbolVisible = Properties.Settings.Default.IsCurrencySymbolVisible;

        public bool IsCurrencySymbolVisible
        {
            get => _isCurrencySymbolVisible;
            set
            {
                if (_isCurrencySymbolVisible != value)
                {
                    _isCurrencySymbolVisible = value;
                    OnPropertyChanged(nameof(IsCurrencySymbolVisible));
                    Properties.Settings.Default.IsCurrencySymbolVisible = value;
                    Properties.Settings.Default.Save(); 
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
