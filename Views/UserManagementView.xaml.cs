using System.Windows.Controls;
using WarehouseManagementSystem.ViewModels;

namespace WarehouseManagementSystem.Views
{
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
        }

        private void PasswordBox_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UserViewModel vm)
                vm.FormPassword = PasswordBox.Password;
        }
    }
}