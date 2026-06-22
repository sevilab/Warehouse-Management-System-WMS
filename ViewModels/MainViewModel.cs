using WarehouseManagementSystem.Patterns;
using WarehouseManagementSystem.Services;
using WarehouseManagementSystem.Interfaces;
using MaterialDesignThemes.Wpf;
using System;

namespace WarehouseManagementSystem.ViewModels
{    //TÜREYEN SINIF SetProperty'yi hazır kullanır
    public class MainViewModel : BaseViewModel, IDisposable
    {
        private readonly AuthService _authService;
        private readonly UIAlertObserver _uiObserver;
        private readonly LogObserver _logObserver;

        public ProductService ProductService { get; private set; }
        public SupplierService SupplierService { get; private set; }
        public ReportService ReportService { get; private set; }
        public LocationService LocationService { get; private set; }
        public IPurchaseOrderService PurchaseOrderService { get; private set; } 

        public DashboardViewModel Dashboard { get; private set; }
        public ProductViewModel Products { get; private set; }
        public SupplierViewModel Suppliers { get; private set; }
        public TransactionViewModel Transactions { get; private set; }
        public UserViewModel UserManagement { get; private set; }
        public LocationViewModel Locations { get; private set; }
        public ReturnViewModel Returns { get; private set; }
        public PurchaseOrderViewModel PurchaseOrders { get; private set; } 

        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value); // BaseViewModel'den gelir
        }

        public SnackbarMessageQueue MainMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public bool IsAdmin => _authService.IsAdmin;

        public RelayCommand ShowDashboardCommand { get; private set; }
        public RelayCommand ShowProductsCommand { get; private set; }
        public RelayCommand ShowSuppliersCommand { get; private set; }
        public RelayCommand ShowTransactionsCommand { get; private set; }
        public RelayCommand ShowUserManagementCommand { get; private set; }
        public RelayCommand ShowLocationsCommand { get; private set; }
        public RelayCommand ShowReturnsCommand { get; private set; }
        public RelayCommand ShowPurchaseOrdersCommand { get; private set; } // Phase 10
        public RelayCommand DismissAlertCommand { get; private set; }

        private string _alertMessage = string.Empty;
        public string AlertMessage
        {
            get => _alertMessage;
            set => SetProperty(ref _alertMessage, value);
        }

        private bool _hasAlert;
        public bool HasAlert
        {
            get => _hasAlert;
            set => SetProperty(ref _hasAlert, value);
        }

        private string _currentUserInfo = string.Empty;
        public string CurrentUserInfo
        {
            get => _currentUserInfo;
            set => SetProperty(ref _currentUserInfo, value);
        }

        public MainViewModel(AuthService authService)
        {
            _authService = authService;
            var warehouse = WarehouseManager.Instance;

            if (authService.CurrentUser != null)
                CurrentUserInfo = $"👤 {authService.CurrentUser.FullName} " +
                                  $"({authService.CurrentUser.Role})";

            _uiObserver = new UIAlertObserver();
            _uiObserver.AlertRaised += (message, level) =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AlertMessage = message;
                    HasAlert = true;
                    MainMessageQueue.Enqueue($"⚠️ {message}");
                }));
            };
            _logObserver = new LogObserver();
            StockAlertSystem.Instance.Subscribe(_uiObserver);
            StockAlertSystem.Instance.Subscribe(_logObserver);

            ProductService = new ProductService(
                warehouse.Products,
                warehouse.Transactions,
                warehouse.Locations,
                warehouse.Returns,
                StockAlertSystem.Instance);

            SupplierService = new SupplierService(
                warehouse.Suppliers,
                warehouse.Products);

            ReportService = new ReportService(
                warehouse.Products,
                warehouse.Suppliers,
                warehouse.Transactions);

            LocationService = new LocationService(
                warehouse.Locations,
                warehouse.Products);

            PurchaseOrderService = new PurchaseOrderService(
                warehouse.PurchaseOrders,
                ProductService,
                SupplierService);

            SeedData.Load(ProductService, SupplierService, LocationService);

            Dashboard = new DashboardViewModel(ProductService, ReportService, PurchaseOrderService, SupplierService, MainMessageQueue);
            Products = new ProductViewModel(ProductService, SupplierService, authService, ReportService, LocationService, MainMessageQueue);
            Suppliers = new SupplierViewModel(SupplierService, authService, ReportService, MainMessageQueue);
            Transactions = new TransactionViewModel(ProductService, ReportService, authService, MainMessageQueue);
            UserManagement = new UserViewModel(warehouse.Users);
            Locations = new LocationViewModel(LocationService, ProductService, MainMessageQueue);
            Returns = new ReturnViewModel(ProductService);
            PurchaseOrders = new PurchaseOrderViewModel(PurchaseOrderService, MainMessageQueue);

            _currentView = Dashboard;

            ShowDashboardCommand = new RelayCommand(_ =>
            {
                Dashboard.Refresh();
                CurrentView = Dashboard;
            });

            ShowProductsCommand = new RelayCommand(_ =>
            {
                Products.Refresh();
                CurrentView = Products;
            });

            ShowSuppliersCommand = new RelayCommand(_ =>
            {
                Suppliers.Refresh();
                CurrentView = Suppliers;
            });
            
            ShowTransactionsCommand = new RelayCommand(_ =>
            {
                Transactions.Refresh();
                CurrentView = Transactions;
            });
            // ViewModels/MainViewModel.cs
            ShowUserManagementCommand = new RelayCommand(_ =>
            {
                CurrentView = UserManagement;
            }, _ => authService.IsAdmin);
            // Sadece Admin ise buton "AKTİF"

            ShowLocationsCommand = new RelayCommand(_ =>
            {
                Locations.Refresh();
                CurrentView = Locations;
            });

            ShowReturnsCommand = new RelayCommand(_ =>
            {
                Returns.RefreshCommand.Execute(null);
                CurrentView = Returns;
            });

            ShowPurchaseOrdersCommand = new RelayCommand(_ =>
            {
                PurchaseOrders.RefreshCommand.Execute(null);
                CurrentView = PurchaseOrders;
            });

            DismissAlertCommand = new RelayCommand(_ => DismissAlert());
        }

        public void DismissAlert()
        {
            AlertMessage = string.Empty;
            HasAlert = false;
        }
        //ViewModels/MainViewModel.cs
        public void Dispose()
        {// Her seferinde aynı nesne gelir
            StockAlertSystem.Instance.Unsubscribe(_uiObserver);
            StockAlertSystem.Instance.Unsubscribe(_logObserver);
        }
    }
}