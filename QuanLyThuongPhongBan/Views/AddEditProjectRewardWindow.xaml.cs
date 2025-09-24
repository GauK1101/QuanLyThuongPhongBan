using System.Windows;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for EditProjectRewardWindow.xaml
    /// </summary>
    public partial class AddEditProjectRewardWindow : Window
    {
        public AddEditProjectRewardWindow()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
