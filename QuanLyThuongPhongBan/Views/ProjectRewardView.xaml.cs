using QuanLyThuongPhongBan.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for ProjectRewardView.xaml
    /// </summary>
    public partial class ProjectRewardView : UserControl
    {
        public ProjectRewardView()
        {
            InitializeComponent();
            Loading();
        }

        private void Loading()
        {
            // Lấy thứ tự cột đã lưu
            var columnOrder = Properties.Settings.Default.ColumnProjectRewardView;

            if (!string.IsNullOrEmpty(columnOrder))
            {
                var orderArray = columnOrder.Split(',').Select(int.Parse).ToArray();

                for (int i = 0; i < orderArray.Length; i++)
                {
                    myDataGrid.Columns[i].DisplayIndex = orderArray[i];
                }
            }
        }

        private void DataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var columnOrder = myDataGrid.Columns
            .Select(c => c.DisplayIndex)
            .ToArray();

            Properties.Settings.Default.ColumnProjectRewardView = string.Join(",", columnOrder);
            Properties.Settings.Default.Save();
        }

        private void chkExpandAll_Checked(object sender, RoutedEventArgs e)
        {
            myDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
        }

        private void chkExpandAll_Unchecked(object sender, RoutedEventArgs e)
        {
            myDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement(myDataGrid, (DependencyObject)sender) as DataGridRow;
            if (row == null) return;

            row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item is TbThuongDuAnChiTiet record)
            {
                if (record.ChuaThanhToan == 0)
                    e.Row.Background = Brushes.LightGreen;

                if (record.DaThanhToan == 0 || record.DaThanhToan == null)
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 235, 236));

                if (record.Tvgp == 0 || record.Tvgp == null)
                {
                    var grid = sender as DataGrid;

                    if (grid != null)
                    {
                        grid.Dispatcher.InvokeAsync(() =>
                        {
                            var tvgpColumn = grid.Columns.FirstOrDefault(c => c.Header?.ToString() == "TVGP");
                            if (tvgpColumn != null)
                            {
                                var cell = GetCell(grid, e.Row, tvgpColumn.DisplayIndex);
                                if (cell != null)
                                {
                                    cell.Background = new SolidColorBrush(Color.FromRgb(255, 227, 186)); // Cam nhạt
                                }
                            }
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
            }
        }

        // Hàm phụ để lấy cell trong DataGridRow
        private DataGridCell? GetCell(DataGrid grid, DataGridRow row, int columnIndex)
        {
            if (row == null) return null;

            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null)
            {
                row.ApplyTemplate();
                presenter = FindVisualChild<DataGridCellsPresenter>(row);
            }

            var cell = presenter?.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
            return cell;
        }

        // Hàm phụ tìm phần tử con theo kiểu
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
