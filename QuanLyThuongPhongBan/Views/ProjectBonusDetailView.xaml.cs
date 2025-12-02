using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.Views
{
    public partial class ProjectBonusDetailView : UserControl
    {
        public ProjectBonusDetailView()
        {
            InitializeComponent();
            DataContext = App.GetService<ProjectBonusDetailViewModel>();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null && DataContext is ProjectBonusDetailViewModel viewModel)
            {
                viewModel.SelectedProjectBonusDetails = new ObservableCollection<ProjectBonusDetail>(dataGrid.SelectedItems.Cast<ProjectBonusDetail>());
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Delay một chút để binding kịp cập nhật
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (DataContext is ProjectBonusDetailViewModel viewModel)
                    {
                        var entity = e.Row.DataContext as ProjectBonusDetail;
                        if (entity != null)
                        {
                            if (viewModel.UppdateCommand.CanExecute(entity))
                            {
                                viewModel.UppdateCommand.Execute(entity);
                            }
                        }
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void ProjectBonusDetailGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (DataContext is ProjectBonusDetailViewModel viewModel)
                {
                    viewModel.PasteToSelectedRowCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}
