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
                    if (cells.Length >= 4 &&
                        DateTime.TryParseExact(cells[3]?.Trim(),
                                               new[] { "dd/MM/yy", "dd/MM/yyyy" },
                                               System.Globalization.CultureInfo.InvariantCulture,
                                               System.Globalization.DateTimeStyles.None,
                                               out DateTime date))
                        newItem.NgayThang = DateOnly.FromDateTime(date);

                    if (cells.Length >= 5 && decimal.TryParse(cells[4], out decimal val))
                        newItem.DoanhThuHopDong = val;
                    if (cells.Length >= 6 && decimal.TryParse(cells[5], out val))
                        newItem.DoanhThuQuyetToan = val;
                    if (cells.Length >= 7 && decimal.TryParse(cells[6], out val))
                        newItem.DoanhThuChuaXuatHoaDon = val;
                    if (cells.Length >= 8 && decimal.TryParse(cells[7], out val))
                        newItem.DoanhThuDaXuatHoaDon = val;
                    if (cells.Length >= 9 && decimal.TryParse(cells[8], out val))
                        newItem.DaThanhToan = val;
                    if (cells.Length >= 10) newItem.GhiChu = cells[9];
                    if (cells.Length >= 11 && decimal.TryParse(cells[10], out val))
                        newItem.Tvgp = val;
                    if (cells.Length >=12 && decimal.TryParse(cells[11], out val))
                        newItem.Hsda = val;
                    if (cells.Length >= 13 && decimal.TryParse(cells[12], out val))
                        newItem.Po = val;
                    if (cells.Length >= 14 && decimal.TryParse(cells[13], out val))
                        newItem.Ktk = val;
                    if (cells.Length >= 15 && decimal.TryParse(cells[14], out val))
                        newItem.Ttdvkt = val;

                    // Thêm vào danh sách binding
                    var list = MyDataGrid.ItemsSource as ObservableCollection<TbThuongDuAnChiTiet>;
                    list?.Add(newItem);
                }
            }
        }

    }
}
