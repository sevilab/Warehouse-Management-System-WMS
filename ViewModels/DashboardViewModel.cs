using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;
using WarehouseManagementSystem.Interfaces;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WarehouseManagementSystem.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly ReportService _reportService;
        private readonly IPurchaseOrderService _orderService; 
        private readonly SupplierService _supplierService; // New
        private readonly ISnackbarMessageQueue _messageQueue;

        // Özet kartlar
        private int _totalProducts;
        public int TotalProducts
        {
            get => _totalProducts;
            set => SetProperty(ref _totalProducts, value);
        }

        private decimal _totalStockValueDecimal;
        public decimal TotalStockValueDecimal
        {
            get => _totalStockValueDecimal;
            set => SetProperty(ref _totalStockValueDecimal, value);
        }

        private int _lowStockCount;
        public int LowStockCount
        {
            get => _lowStockCount;
            set => SetProperty(ref _lowStockCount, value);
        }

        private int _criticalStockCount;
        public int CriticalStockCount
        {
            get => _criticalStockCount;
            set => SetProperty(ref _criticalStockCount, value);
        }

        private int _totalSuppliers;
        public int TotalSuppliers
        {
            get => _totalSuppliers;
            set => SetProperty(ref _totalSuppliers, value);
        }

        private int _totalTransactions;
        public int TotalTransactions
        {
            get => _totalTransactions;
            set => SetProperty(ref _totalTransactions, value);
        }

        // Sipariş Formu State (Phase 10 Enhanced)
        private bool _isOrderDialogOpen;
        public bool IsOrderDialogOpen
        {
            get => _isOrderDialogOpen;
            set => SetProperty(ref _isOrderDialogOpen, value);
        }

        private Product? _orderProduct;
        public Product? OrderProduct
        {
            get => _orderProduct;
            set => SetProperty(ref _orderProduct, value);
        }

        private string _orderQuantityText = "1";
        public string OrderQuantityText
        {
            get => _orderQuantityText;
            set => SetProperty(ref _orderQuantityText, value);
        }
        
        private string _orderUnitPriceText = "0";
        public string OrderUnitPriceText
        {
            get => _orderUnitPriceText;
            set => SetProperty(ref _orderUnitPriceText, value);
        }

        private Supplier? _selectedSupplier;
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        private List<Supplier> _suppliers = new();
        public List<Supplier> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        private List<Product> _lowStockProducts = new();
        public List<Product> LowStockProducts
        {
            get => _lowStockProducts;
            set => SetProperty(ref _lowStockProducts, value);
        }

        public RelayCommand OpenOrderDialogCommand { get; }
        public RelayCommand CloseOrderDialogCommand { get; }
        public RelayCommand SaveOrderCommand { get; }

        public DashboardViewModel(ProductService productService,
                                  ReportService reportService,
                                  IPurchaseOrderService orderService,
                                  SupplierService supplierService,
                                  ISnackbarMessageQueue messageQueue)
        {
            _productService = productService;
            _reportService = reportService;
            _orderService = orderService;
            _supplierService = supplierService;
            _messageQueue = messageQueue;

            OpenOrderDialogCommand = new RelayCommand(p =>
            {
                var product = p as Product;
                OrderProduct = product;
                OrderQuantityText = (product?.MinStockLevel * 2 ?? 10).ToString();
                OrderUnitPriceText = product?.Price.ToString() ?? "0";
                
                // Varsayılan tedarikçiyi seç
                if (product != null)
                {
                    SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
                }
                
                IsOrderDialogOpen = true;
            });

            CloseOrderDialogCommand = new RelayCommand(_ => IsOrderDialogOpen = false);

            SaveOrderCommand = new RelayCommand(_ =>
            {
                if (!int.TryParse(OrderQuantityText, out int quantity) || quantity <= 0)
                {
                    _messageQueue.Enqueue("Lütfen geçerli bir sipariş miktarı girin.");
                    return;
                }

                if (!decimal.TryParse(OrderUnitPriceText, out decimal unitPrice) || unitPrice < 0)
                {
                    _messageQueue.Enqueue("Lütfen geçerli bir birim fiyat girin.");
                    return;
                }

                if (OrderProduct == null || SelectedSupplier == null) 
                {
                    if (SelectedSupplier == null) _messageQueue.Enqueue("Lütfen bir tedarikçi seçin.");
                    return;
                }

                try
                {
                    _orderService.CreateOrder(OrderProduct.Id, quantity, SelectedSupplier.Id, unitPrice);
                    _messageQueue.Enqueue($"{OrderProduct.Name} için {SelectedSupplier.Name} üzerinden {quantity} adet sipariş oluşturuldu.");
                    IsOrderDialogOpen = false;
                }
                catch (Exception ex)
                {
                    _messageQueue.Enqueue($"Hata: {ex.Message}");
                }
            });

            Refresh();
        }

        public void Refresh()
        {
            var summary = _reportService.GetStockSummary();
            TotalProducts = summary.TotalProducts;
            TotalStockValueDecimal = summary.TotalStockValue;
            LowStockCount = summary.LowStockCount;
            CriticalStockCount = summary.CriticalStockCount;
            TotalSuppliers = summary.TotalSuppliers;
            TotalTransactions = summary.TotalTransactions;

            LowStockProducts = _productService.GetLowStockProducts().ToList();
            Suppliers = _supplierService.GetAllSuppliers().ToList();
        }
    }
}