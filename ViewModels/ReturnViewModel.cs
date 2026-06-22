using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using System.Text;
using System.IO;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Patterns;

namespace WarehouseManagementSystem.ViewModels
{
    public class ReturnViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        
        public ObservableCollection<Product> AvailableProducts { get; set; } = new();
        public ObservableCollection<Return> ReturnsHistory { get; set; } = new();
        public Array Reasons => Enum.GetValues(typeof(ReturnReason));
        public Array Actions => Enum.GetValues(typeof(ReturnAction));

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private ReturnReason _selectedReason = ReturnReason.Vazgecildi;
        public ReturnReason SelectedReason
        {
            get => _selectedReason;
            set => SetProperty(ref _selectedReason, value);
        }

        private ReturnAction _selectedAction = ReturnAction.StokaGeriAl;
        public ReturnAction SelectedAction
        {
            get => _selectedAction;
            set => SetProperty(ref _selectedAction, value);
        }

        private string _description = "";
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _searchProductText = "";
        public string SearchProductText
        {
            get => _searchProductText;
            set
            {
                if (SetProperty(ref _searchProductText, value))
                {
                    FilterProducts();
                }
            }
        }

        public ICommand ProcessReturnCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand RefreshCommand { get; }

        public ReturnViewModel(IProductService productService)
        {
            _productService = productService;
            
            ProcessReturnCommand = new RelayCommand(ProcessReturn, can => SelectedProduct != null && Quantity > 0);
            ExportToCsvCommand = new RelayCommand(ExportToCsv);
            RefreshCommand = new RelayCommand(_ => LoadData());

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                AvailableProducts.Clear();
                foreach (var p in _productService.GetProductsWithDetails())
                    AvailableProducts.Add(p);

                ReturnsHistory.Clear();
                foreach (var r in _productService.GetReturns())
                    ReturnsHistory.Add(r);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"İade verileri yüklenirken hata: {ex.Message}");
            }
        }

        private void FilterProducts()
        {
            var filtered = _productService.GetProductsWithDetails()
                .Where(p => string.IsNullOrWhiteSpace(SearchProductText) || 
                           p.Name.Contains(SearchProductText, StringComparison.OrdinalIgnoreCase) ||
                           p.SKU.Contains(SearchProductText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AvailableProducts.Clear();
            foreach (var p in filtered)
                AvailableProducts.Add(p);
        }

        private void ProcessReturn(object? parameter)
        {
            if (SelectedProduct == null) return;

            try
            {
                var returnItem = new Return
                {
                    ProductId = SelectedProduct.Id,
                    ProductName = SelectedProduct.Name,
                    SKU = SelectedProduct.SKU,
                    Quantity = Quantity,
                    Reason = SelectedReason,
                    Action = SelectedAction,
                    Description = Description,
                    CreatedAt = DateTime.Now
                };

                _productService.RegisterReturn(returnItem);
                
                // Reset form
                Quantity = 1;
                Description = "";
                
                LoadData();
                // Snackbar bildirimi gerekebilir ama serviste bildirim yoksa mesaj gösterilebilir
            }
            catch (Exception)
            {
                // Hata yönetimi
            }
        }

        private void ExportToCsv(object? parameter)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "CSV Dosyası (*.csv)|*.csv",
                FileName = $"Iade_Raporu_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Tarih;Urun;SKU;Miktar;Neden;Islem;Aciklama");

                foreach (var r in ReturnsHistory)
                {
                    sb.AppendLine($"{r.CreatedAt:dd.MM.yyyy HH:mm};{r.ProductName};{r.SKU};{r.Quantity};{r.ReasonDisplay};{r.ActionDisplay};\"{r.Description}\"");
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
