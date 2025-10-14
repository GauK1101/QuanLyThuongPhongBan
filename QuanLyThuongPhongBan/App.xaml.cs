using QuanLyThuongPhongBan.ModelSettings;
using System.Windows;

namespace QuanLyThuongPhongBan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppSettings Settings { get; } = new AppSettings();
    }
}
