using QuanLyThuongPhongBan.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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

        private void Title_Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void MyDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PasteFromClipboard();
                e.Handled = true;
            }
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                string[] lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] cells = line.Split('\t'); // Excel copy = tab-delimited

                    // Tạo bản ghi mới
                    var newItem = new TbThuongDuAnChiTiet();

                    if (cells.Length >= 1) newItem.ChuDauTu = cells[0];
                    if (cells.Length >= 2) newItem.HopDongSo = cells[1];
                    if (cells.Length >= 3) newItem.DuAn = cells[2];
                    if (cells.Length >= 4 && DateTime.TryParse(cells[3], out DateTime date))
                        newItem.NgayThang = DateOnly.FromDateTime(date);
                    if (cells.Length >= 5 && decimal.TryParse(cells[4], out decimal val))
                        newItem.DoanhThuHopDong = val;

                    // Thêm vào danh sách binding
                    var list = MyDataGrid.ItemsSource as ObservableCollection<TbThuongDuAnChiTiet>;
                    list?.Add(newItem);
                }
            }
        }

    }
}
