using QuanLyThuongPhongBan.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for SMBRewardView.xaml
    /// </summary>
    public partial class SmbRewardView : UserControl
    {
        public SmbRewardView()
        {
            InitializeComponent();
            DataContext = App.GetService<SmbRewardViewModel>();
        }

        private void AnyCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && DataContext is SmbRewardViewModel vm)
            {
                if (e.EditingElement is TextBox textBox)
                {
                    var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                }

                Dispatcher.BeginInvoke(() =>
                {
                    SmbBonus bonusToUpdate = null;

                    if (e.Row.DataContext is SmbBonus entity)
                        bonusToUpdate = entity;
                    else if (e.Row.DataContext is SmbTeamBonus teamBonus)
                        bonusToUpdate = teamBonus.SmbBonus;

                    if (bonusToUpdate != null)
                        vm.UppdateCommand?.Execute(bonusToUpdate);

                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null && DataContext is SmbRewardViewModel vm)
            {
                vm.SelectedSmbBonuses = new ObservableCollection<SmbBonus>(dataGrid.SelectedItems.Cast<SmbBonus>());
            }
        }
    }
}
