using System.Collections.ObjectModel;
using System.Linq;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using MaterialDesignThemes.Wpf;

namespace WarehouseManagementSystem.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private readonly IPurchaseOrderService _orderService;
        private readonly ISnackbarMessageQueue _messageQueue;
        private volatile bool _isLoading;

        private ObservableCollection<PurchaseOrder> _orders = new();
        public ObservableCollection<PurchaseOrder> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        private bool _showOnlyPending = true;
        public bool ShowOnlyPending
        {
            get => _showOnlyPending;
            set
            {
                if (SetProperty(ref _showOnlyPending, value))
                {
                    _ = LoadOrdersAsync().ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "error_log.txt"), t.Exception.ToString());
                            System.Diagnostics.Debug.WriteLine($"LoadOrders FAILED: {t.Exception}");
                        }
                    }, System.Threading.Tasks.TaskScheduler.Default);
                }
            }
        }

        public RelayCommand ReceiveOrderCommand { get; }
        public RelayCommand DeleteOrderCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public PurchaseOrderViewModel(IPurchaseOrderService orderService, ISnackbarMessageQueue messageQueue)
        {
            _orderService = orderService;
            _messageQueue = messageQueue;

            ReceiveOrderCommand = new RelayCommand(async p =>
            {
                var order = p as PurchaseOrder;
                if (order == null) return;
                try
                {
                    _orderService.MarkAsReceived(order.Id);
                    _messageQueue.Enqueue($"{order.ProductName} teslim alındı ve stok güncellendi.");
                    await LoadOrdersAsync();
                }
                catch (System.Exception ex)
                {
                    _messageQueue.Enqueue($"Hata: {ex.Message}");
                }
            });

            DeleteOrderCommand = new RelayCommand(async p =>
            {
                var order = p as PurchaseOrder;
                if (order == null) return;
                _orderService.DeleteOrder(order.Id);
                _messageQueue.Enqueue("Sipariş silindi.");
                await LoadOrdersAsync();
            });

            RefreshCommand = new RelayCommand(async _ => await LoadOrdersAsync());
        }

        public async System.Threading.Tasks.Task LoadOrdersAsync()
        {
            if (_isLoading) return;
            _isLoading = true;
            try
            {
                System.Diagnostics.Debug.WriteLine($"PurchaseOrderViewModel: Loading orders... PendingOnly={ShowOnlyPending}");
                var dataList = ShowOnlyPending 
                    ? _orderService.GetPendingOrders().ToList() 
                    : _orderService.GetAllOrders().ToList();
                System.Diagnostics.Debug.WriteLine($"PurchaseOrderViewModel: Found {dataList.Count} orders.");

                Orders.Clear();
                foreach (var order in dataList)
                {
                    Orders.Add(order);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PurchaseOrderViewModel ERROR: {ex.Message}");
                System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "error_log.txt"), ex.ToString());
                _messageQueue.Enqueue($"Hata (Liste Yükleme): {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
