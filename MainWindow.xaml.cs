using System.Windows;
using WarehouseManagementSystem.ViewModels;
using WarehouseManagementSystem.Views;

namespace WarehouseManagementSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(App.AuthService!);
        }

        private void DismissAlert_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.DismissAlert();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.AuthService?.Logout();
            var loginViewModel = new LoginViewModel(App.AuthService!);
            var loginWindow = new LoginWindow(loginViewModel);

            this.Hide(); // Ana ekranı gizle, sadece login görünsün

            bool? result = loginWindow.ShowDialog();
            if (result == true)
            {
                // Eski ViewModel'i dispose et (observer leak'i önler)
                var oldContext = this.DataContext as MainViewModel;
                this.DataContext = null;
                oldContext?.Dispose();

                // Mevcut pencere içine yeni ViewModel ata ve tekrar göster
                this.DataContext = new MainViewModel(App.AuthService!);
                this.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}