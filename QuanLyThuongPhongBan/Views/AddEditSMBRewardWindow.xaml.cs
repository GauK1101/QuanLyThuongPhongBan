using System.Windows;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for AddEditSMBRewardWindow.xaml
    /// </summary>
    public partial class AddEditSMBRewardWindow : Window
    {
        public AddEditSMBRewardWindow()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
