using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for SMBRewardView.xaml
    /// </summary>
    public partial class SMBRewardView : UserControl
    {
        public SMBRewardView()
        {
            InitializeComponent();
            Loading();
        }

        private void Loading()
        {
            // Lấy thứ tự cột đã lưu
            var columnOrder = Properties.Settings.Default.ColumnSMBtRewardView;

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

            Properties.Settings.Default.ColumnSMBtRewardView = string.Join(",", columnOrder);
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

    }
}
