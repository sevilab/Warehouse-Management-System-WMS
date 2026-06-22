using System.Windows.Controls;

namespace WarehouseManagementSystem.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        private void ListViewItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.ProductViewModel vm &&
                sender is ListViewItem item &&
                item.DataContext is Models.Product product)
            {
                vm.SelectProductCommand.Execute(product);
            }
        }
    }
}