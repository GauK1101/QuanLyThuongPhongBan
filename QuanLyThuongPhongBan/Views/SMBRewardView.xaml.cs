using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
            }
        }
    }
}
