using System.Collections.ObjectModel;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace WarehouseManagementSystem.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        public bool IsAdmin => _authService.IsAdmin;
        private readonly ProductService _productService;
        private readonly SupplierService _supplierService;
        private readonly ReportService _reportService;
        private readonly LocationService _locationService;
        private readonly ISnackbarMessageQueue _messageQueue;

        private List<Product> _allProducts = new();

        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private ObservableCollection<LocationOption> _locationsSet = new();
        public ObservableCollection<LocationOption> LocationsSet
        {
            get => _locationsSet;
            set => SetProperty(ref _locationsSet, value);
        }

        private LocationOption? _selectedLocation;
        public LocationOption? SelectedLocation
        {
            get => _selectedLocation;
            set => SetProperty(ref _selectedLocation, value);
        }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                SetProperty(ref _selectedProduct, value);
                if (value != null) FillFormFromProduct(value);
            }
        }

        // Arama
        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                SetProperty(ref _searchKeyword, value);
                ApplyFilters();
            }
        }

        // Kategori filtre
        private string _selectedCategoryFilter = "Tümü";
        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                SetProperty(ref _selectedCategoryFilter, value);
                ApplyFilters();
            }
        }

        // Stok durum filtre
        private string _selectedStockFilter = "Tümü";
        public string SelectedStockFilter
        {
            get => _selectedStockFilter;
            set
            {
                SetProperty(ref _selectedStockFilter, value);
                ApplyFilters();
            }
        }

        // Sıralama
        private string _selectedSortOption = "Ad (A-Z)";
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                SetProperty(ref _selectedSortOption, value);
                ApplyFilters();
            }
        }

        // Filtre listeleri
        public List<string> CategoryFilters { get; private set; } = new();
        public List<string> StockFilters { get; } = new()
        {
            "Tümü", "Normal", "Düşük", "Kritik"
        };
        public List<string> SortOptions { get; } = new()
        {
            "Ad (A-Z)", "Ad (Z-A)",
            "Fiyat (Düşük-Yüksek)", "Fiyat (Yüksek-Düşük)",
            "Stok (Düşük-Yüksek)", "Stok (Yüksek-Düşük)"
        };

        // Form alanları
        private string _formName = string.Empty;
        public string FormName
        {
            get => _formName;
            set => SetProperty(ref _formName, value);
        }

        private string _formSKU = string.Empty;
        public string FormSKU
        {
            get => _formSKU;
            set => SetProperty(ref _formSKU, value);
        }

        private string _formDescription = string.Empty;
        public string FormDescription
        {
            get => _formDescription;
            set => SetProperty(ref _formDescription, value);
        }

        private string _formPriceText = "0";
        public string FormPriceText
        {
            get => _formPriceText;
            set
            {
                if (SetProperty(ref _formPriceText, value))
                {
                    if (decimal.TryParse(value, out decimal p))
                        _formPrice = p;
                }
            }
        }
        private decimal _formPrice;

        private string _formMinStockText = "0";
        public string FormMinStockText
        {
            get => _formMinStockText;
            set
            {
                if (SetProperty(ref _formMinStockText, value))
                {
                    if (int.TryParse(value, out int q))
                        _formMinStock = q;
                }
            }
        }
        private int _formMinStock;

        private string _formUnit = "Adet";
        public string FormUnit
        {
            get => _formUnit;
            set => SetProperty(ref _formUnit, value);
        }

        private string _formColor = string.Empty;
        public string FormColor
        {
            get => _formColor;
            set => SetProperty(ref _formColor, value);
        }

        private string _formSize = string.Empty;
        public string FormSize
        {
            get => _formSize;
            set => SetProperty(ref _formSize, value);
        }

        private Category _formCategory;
        public Category FormCategory
        {
            get => _formCategory;
            set => SetProperty(ref _formCategory, value);
        }

        private Supplier? _formSupplier;
        public Supplier? FormSupplier
        {
            get => _formSupplier;
            set => SetProperty(ref _formSupplier, value);
        }

        // Stok işlem alanları
        private string _stockQuantityText = "0";
        public string StockQuantityText
        {
            get => _stockQuantityText;
            set
            {
                if (SetProperty(ref _stockQuantityText, value))
                {
                    if (int.TryParse(value, out int q))
                        _stockQuantity = q;
                }
            }
        }
        private int _stockQuantity;

        private string _stockUnitPriceText = "0";
        public string StockUnitPriceText
        {
            get => _stockUnitPriceText;
            set
            {
                if (SetProperty(ref _stockUnitPriceText, value))
                {
                    if (decimal.TryParse(value, out decimal p))
                        _stockUnitPrice = p;
                }
            }
        }
        private decimal _stockUnitPrice;

        private string _stockDescription = string.Empty;
        public string StockDescription
        {
            get => _stockDescription;
            set => SetProperty(ref _stockDescription, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isStatusError;
        public bool IsStatusError
        {
            get => _isStatusError;
            set => SetProperty(ref _isStatusError, value);
        }

        // Sonuç sayısı
        private int _resultCount;
        public int ResultCount
        {
            get => _resultCount;
            set => SetProperty(ref _resultCount, value);
        }

        // Ürün Bul (Search) - Dashboard'dan taşındı
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchProducts();
                }
            }
        }

        private ObservableCollection<ProductSearchResult> _searchResults = new();
        public ObservableCollection<ProductSearchResult> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public RelayCommand SearchCommand { get; }

        // Komutlar
        public RelayCommand AddProductCommand { get; private set; }
        public RelayCommand UpdateProductCommand { get; private set; }
        public RelayCommand DeleteProductCommand { get; private set; }
        public RelayCommand SelectProductCommand { get; private set; }
        public RelayCommand StockInCommand { get; private set; }
        public RelayCommand StockOutCommand { get; private set; }
        public RelayCommand ClearFormCommand { get; private set; }
        public RelayCommand ClearFiltersCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }

        public List<Category> Categories { get; } = Enum.GetValues<Category>().ToList();
        public List<Supplier> Suppliers { get; private set; } = new();

        public ProductViewModel(ProductService productService,
                                SupplierService supplierService,
                                AuthService authService,
                                ReportService reportService,
                                LocationService locationService,
                                ISnackbarMessageQueue messageQueue)
        {
            _productService = productService;
            _supplierService = supplierService;
            _authService = authService;
            _reportService = reportService;
            _locationService = locationService;
            _messageQueue = messageQueue;

            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => IsAdmin);
            UpdateProductCommand = new RelayCommand(_ => UpdateProduct(),
                _ => SelectedProduct != null && IsAdmin);
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct(),
                _ => SelectedProduct != null && IsAdmin);
            SelectProductCommand = new RelayCommand(p =>
            {
                if (p is Product product)
                {
                    SelectedProduct = product;
                    FillFormFromProduct(product);
                }
            });
            StockInCommand = new RelayCommand(_ => DoStockIn(),
                _ => SelectedProduct != null && _stockQuantity > 0);
            StockOutCommand = new RelayCommand(_ => DoStockOut(),
                _ => SelectedProduct != null && _stockQuantity > 0);
            ClearFormCommand = new RelayCommand(_ => ClearForm());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ExportCommand = new RelayCommand(_ => ExportToCsv());
            SearchCommand = new RelayCommand(_ => SearchProducts());

            Refresh();
        }

        private void SearchProducts()
        {
            SearchResults.Clear();
            if (string.IsNullOrWhiteSpace(SearchText) || SearchText.Length < 2) return;

            var kw = SearchText.ToLower();
            var products = _productService.GetProductsWithDetails()
                .Where(p => p.Name.ToLower().Contains(kw) || 
                            p.SKU.ToLower().Contains(kw))
                .ToList();

            foreach (var p in products)
            {
                SearchResults.Add(new ProductSearchResult
                {
                    ProductName = p.Name,
                    SKU = p.SKU,
                    Stock = $"{p.StockQuantity} {p.Unit}",
                    LocationInfo = p.LocationName ?? "Raf Atanmamış",
                    FullAddress = p.LocationId.HasValue ? $"{p.LocationName} ({p.Category})" : "Yok",
                    Color = string.IsNullOrWhiteSpace(p.Color) ? "-" : p.Color,
                    Size = string.IsNullOrWhiteSpace(p.Size) ? "Standart" : p.Size
                });
            }
        }

        private void ExportToCsv()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Dosyası (*.csv)|*.csv",
                FileName = $"Urun_Listesi_{DateTime.Now:ddMMyyyy_HHmm}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var csvContent = _reportService.GetProductsCsv();
                    File.WriteAllText(saveDialog.FileName, csvContent, Encoding.UTF8);
                    _messageQueue.Enqueue("Rapor başarıyla kaydedildi.");
                }
                catch (Exception ex)
                {
                    _messageQueue.Enqueue("Hata: " + ex.Message);
                }
            }
        }

        public void Refresh()
        {
            _allProducts = _productService.GetProductsWithDetails().ToList();
            Suppliers = _supplierService.GetAllSuppliers().ToList();
            OnPropertyChanged(nameof(Suppliers));

            var allLocs = _locationService.GetAllLocations();
            var options = allLocs.Select(l => new LocationOption
            {
                Id = l.Id,
                Name = l.Name,
                Zone = l.Zone,
                IsEmpty = !_allProducts.Any(p => p.LocationId == l.Id && p.StockQuantity > 0)
            }).OrderByDescending(o => o.IsEmpty).ThenBy(o => o.Name).ToList();

            LocationsSet = new ObservableCollection<LocationOption>(options);
            OnPropertyChanged(nameof(LocationsSet));

            // Kategori filtrelerini güncelle
            CategoryFilters = new List<string> { "Tümü" };
            CategoryFilters.AddRange(_allProducts
                .Select(p => p.Category.ToString())
                .Distinct()
                .OrderBy(c => c));
            OnPropertyChanged(nameof(CategoryFilters));

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var kw = SearchKeyword.ToLower();
                filtered = filtered.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    p.SKU.ToLower().Contains(kw));
            }

            // Kategori filtresi
            if (SelectedCategoryFilter != "Tümü")
            {
                filtered = filtered.Where(p =>
                    p.Category.ToString() == SelectedCategoryFilter);
            }

            // Stok durum filtresi
            filtered = SelectedStockFilter switch
            {
                "Normal" => filtered.Where(p => p.AlertLevel == AlertLevel.Normal),
                "Düşük" => filtered.Where(p => p.AlertLevel == AlertLevel.Düşük),
                "Kritik" => filtered.Where(p => p.AlertLevel == AlertLevel.Kritik),
                _ => filtered
            };

            // Sıralama
            filtered = SelectedSortOption switch
            {
                "Ad (A-Z)" => filtered.OrderBy(p => p.Name),
                "Ad (Z-A)" => filtered.OrderByDescending(p => p.Name),
                "Fiyat (Düşük-Yüksek)" => filtered.OrderBy(p => p.Price),
                "Fiyat (Yüksek-Düşük)" => filtered.OrderByDescending(p => p.Price),
                "Stok (Düşük-Yüksek)" => filtered.OrderBy(p => p.StockQuantity),
                "Stok (Yüksek-Düşük)" => filtered.OrderByDescending(p => p.StockQuantity),
                _ => filtered.OrderBy(p => p.Name)
            };

            var result = filtered.ToList();
            foreach (var p in result)
            {
                p.SupplierName = Suppliers.FirstOrDefault(s => s.Id == p.SupplierId)?.Name ?? "Bilinmiyor";
                if (p.LocationId.HasValue)
                {
                    var loc = LocationsSet.FirstOrDefault(l => l.Id == p.LocationId);
                    if (loc != null)
                        p.LocationName = loc.Name;
                }
            }
            Products = new ObservableCollection<Product>(result);
            ResultCount = result.Count;
        }

        private void ClearFilters()
        {
            SearchKeyword = string.Empty;
            SelectedCategoryFilter = "Tümü";
            SelectedStockFilter = "Tümü";
            SelectedSortOption = "Ad (A-Z)";
        }

        private void FillFormFromProduct(Product product)
        {
            FormName = product.Name;
            FormSKU = product.SKU;
            FormDescription = product.Description;
            FormPriceText = product.Price.ToString();
            FormMinStockText = product.MinStockLevel.ToString();
            FormUnit = product.Unit;
            FormCategory = product.Category;
            FormColor = product.Color;
            FormSize = product.Size;
            FormSupplier = Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
            SelectedLocation = LocationsSet.FirstOrDefault(l => l.Id == product.LocationId);
            StockUnitPriceText = product.Price.ToString();
            StatusMessage = string.Empty;
        }

        private void ClearForm()
        {
            FormName = string.Empty;
            FormSKU = string.Empty;
            FormDescription = string.Empty;
            FormPriceText = "0";
            FormMinStockText = "0";
            FormUnit = "Adet";
            FormCategory = Categories.First();
            FormColor = string.Empty;
            FormSize = string.Empty;
            FormSupplier = null;
            StockQuantityText = "0";
            StockDescription = string.Empty;
            SelectedProduct = null;
            StatusMessage = string.Empty;
        }

        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(FormName))
            { SetError("Ürün adı boş olamaz!"); return; }
            if (string.IsNullOrWhiteSpace(FormSKU))
            { SetError("SKU kodu boş olamaz!"); return; }
            if (_formPrice <= 0)
            { SetError("Fiyat sıfırdan büyük olmalıdır!"); return; }
            if (_formMinStock < 0)
            { SetError("Min. stok seviyesi negatif olamaz!"); return; }
            if (FormSupplier == null)
            { SetError("Lütfen bir tedarikçi seçin!"); return; }

            try
            {
                var product = new Product(FormName, FormSKU, _formPrice,
                    _formMinStock, FormCategory, FormSupplier.Id,
                    SelectedLocation?.Id, FormUnit, FormDescription,
                    FormColor, FormSize);
                _productService.AddProduct(product);
                SetSuccess($"✅ '{FormName}' ürünü eklendi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void UpdateProduct()
        {
            if (SelectedProduct == null) return;
            if (string.IsNullOrWhiteSpace(FormName))
            { SetError("Ürün adı boş olamaz!"); return; }
            if (_formPrice <= 0)
            { SetError("Fiyat sıfırdan büyük olmalıdır!"); return; }

            try
            {
                _productService.UpdateProduct(SelectedProduct.Id,
                    FormName, FormDescription, _formMinStock,
                    _formPrice, FormCategory, SelectedLocation?.Id,
                    FormColor, FormSize);
                SetSuccess($"✅ '{FormName}' güncellendi.");
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;
            try
            {
                string name = SelectedProduct.Name;
                _productService.DeleteProduct(SelectedProduct.Id);
                SetSuccess($"✅ '{name}' silindi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void DoStockIn()
        {
            if (SelectedProduct == null) return;
            if (_stockQuantity <= 0)
            { SetError("Miktar sıfırdan büyük olmalıdır!"); return; }

            try
            {
                _productService.StockIn(SelectedProduct.Id,
                    _stockQuantity, _stockUnitPrice, StockDescription);
                SetSuccess($"✅ {_stockQuantity} adet stok girişi yapıldı.");
                StockQuantityText = "0";
                StockDescription = string.Empty;
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void DoStockOut()
        {
            if (SelectedProduct == null) return;
            if (_stockQuantity <= 0)
            { SetError("Miktar sıfırdan büyük olmalıdır!"); return; }

            try
            {
                _productService.StockOut(SelectedProduct.Id,
                    _stockQuantity, StockDescription);
                SetSuccess($"✅ {_stockQuantity} adet stok çıkışı yapıldı.");
                StockQuantityText = "0";
                StockDescription = string.Empty;
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void SetError(string message)
        {
            StatusMessage = "❌ " + message;
            IsStatusError = true;
            _messageQueue.Enqueue(message);
        }

        private void SetSuccess(string message)
        {
            StatusMessage = message;
            IsStatusError = false;
            _messageQueue.Enqueue(message);
        }
    }
    public class LocationOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public bool IsEmpty { get; set; }
        public string DisplayName => IsEmpty ? $"[BOŞ] {Name} ({Zone})" : $"{Name} ({Zone})";
    }

    public class ProductSearchResult
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Stock { get; set; } = string.Empty;
        public string LocationInfo { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
    }
}