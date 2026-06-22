using System.Windows.Controls;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Views
{
    public partial class SupplierView : UserControl
    {
        public SupplierView()
        {
            InitializeComponent();
        }

        private void ListViewItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is SupplierViewModel vm &&
                sender is ListViewItem item &&
                item.DataContext is Supplier supplier)
            {
                vm.SelectSupplierCommand.Execute(supplier);
            }
        }
    }
}