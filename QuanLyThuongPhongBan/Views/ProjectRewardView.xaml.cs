using System.Windows.Controls;

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
    }
}
