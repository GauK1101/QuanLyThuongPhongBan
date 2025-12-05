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
            
            if (DataContext is SmbRewardViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged; // cách đẹp hơn: tách ra hàm riêng
            }
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

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SmbRewardViewModel.SelectedSmbBonus)
                && sender is SmbRewardViewModel vm
                && vm.SelectedSmbBonus != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    myDataGrid.ScrollIntoView(vm.SelectedSmbBonus);
                    myDataGrid.UpdateLayout(); // đảm bảo scroll chính xác
                });
            }
        }
    }
}
