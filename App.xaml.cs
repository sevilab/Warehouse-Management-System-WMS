using System.Windows;
using WarehouseManagementSystem.Converters;
using WarehouseManagementSystem.Patterns;
using WarehouseManagementSystem.Services;
using WarehouseManagementSystem.ViewModels;
using WarehouseManagementSystem.Views;

namespace WarehouseManagementSystem
{
    public partial class App : Application
    {
        public static AuthService? AuthService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Tüm iş parçacıkları için hata yakalayıcılar
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
                base.OnStartup(e);

                // Auth servisi başlat
                AuthService = new AuthService(WarehouseManager.Instance.Users);

                // Login ekranı göster
                var loginViewModel = new LoginViewModel(AuthService);
                var loginWindow = new LoginWindow(loginViewModel);

                bool? result = loginWindow.ShowDialog();

                if (result == true)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    Shutdown();
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Uygulama başlatılamadı!\n\nHata: {ex.Message}\n\nDetay: {ex.InnerException?.Message}", 
                    "Sistem Hatası", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                
                System.Diagnostics.Debug.WriteLine($"CRITICAL STARTUP ERROR: {ex}");
                Shutdown();
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.IO.File.WriteAllText("crash.log", $"UI Hatası: {e.Exception}");
            System.Windows.MessageBox.Show($"UI Hatası oluştu!\n\nHata: {e.Exception.Message}\n\nDetay: {e.Exception.StackTrace}", 
                "Kritik UI Hatası", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            
            System.Diagnostics.Debug.WriteLine($"UNHANDLED UI EXCEPTION: {e.Exception}");
            e.Handled = true;
            Shutdown();
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as System.Exception;
            System.IO.File.WriteAllText("crash.log", $"Sistem Hatası: {ex}");
            System.Windows.MessageBox.Show($"Sistem Hatası oluştu!\n\nHata: {ex?.Message}\n\nDetay: {ex?.StackTrace}", 
                "Kritik Sistem Hatası", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            
            System.Diagnostics.Debug.WriteLine($"UNHANDLED DOMAIN EXCEPTION: {ex}");
            // Domain exception sonrası uygulama mecburen kapanır
        }

        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"UNOBSERVED TASK EXCEPTION: {e.Exception}");
            e.SetObserved(); // Uygulamanın çökmesini engellemeye çalış
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (WarehouseManager.Instance != null && WarehouseManager.Instance.DbContext != null)
                WarehouseManager.Instance.DbContext.Dispose();
            base.OnExit(e);
        }
    }
}