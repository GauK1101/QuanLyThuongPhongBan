using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void myDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            // Toggle trạng thái hiển thị chi tiết
            if (row.DetailsVisibility == Visibility.Visible)
                row.DetailsVisibility = Visibility.Collapsed;
            else
                row.DetailsVisibility = Visibility.Visible;
        }
    }
}
