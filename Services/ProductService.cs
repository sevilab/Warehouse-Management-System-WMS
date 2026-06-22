using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Patterns;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.Services
{
    // OOP: Encapsulation + Abstraction
    // Interface'e bağımlılık (Dependency Inversion) ✅
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly TransactionRepository _transactionRepository;
        private readonly LocationRepository _locationRepository;
        private readonly ReturnRepository _returnRepository;
        private readonly StockAlertSystem _alertSystem;

        public ProductService(ProductRepository productRepository,
                              TransactionRepository transactionRepository,
                              LocationRepository locationRepository,
                              ReturnRepository returnRepository,
                              StockAlertSystem alertSystem)
        {
            _productRepository = productRepository;
            _transactionRepository = transactionRepository;
            _locationRepository = locationRepository;
            _returnRepository = returnRepository;
            _alertSystem = alertSystem;
        }

        public void AddProduct(Product product)
        {
            // SKU benzersizlik kontrolü
            var existing = _productRepository.GetBySKU(product.SKU);
            if (existing != null)
                throw new InvalidOperationException($"'{product.SKU}' SKU'su zaten mevcut.");

            // Lokasyon Kısıtları
            if (product.LocationId.HasValue)
            {
                ValidateLocationConstraints(product.LocationId.Value, product.Category, product.Id);
            }

            _productRepository.Add(product);
        }

        public void UpdateProduct(int id, string name, string description,
                                  int minStockLevel, decimal price, Category category, int? locationId,
                                  string color, string size)
        {
            var product = GetProductById(id)
                ?? throw new KeyNotFoundException($"Ürün bulunamadı. Id: {id}");

            // Lokasyon Kısıtları
            if (locationId.HasValue)
            {
                ValidateLocationConstraints(locationId.Value, category, id);
            }

            product.UpdateInfo(name, description, minStockLevel, color, size);
            product.UpdatePrice(price);
            product.Category = category;
            product.LocationId = locationId;
            _productRepository.Update(product);
        }

        private void ValidateLocationConstraints(int locationId, Category newCategory, int currentProductId)
        {
            var location = _locationRepository.GetById(locationId)
                ?? throw new KeyNotFoundException($"Lokasyon bulunamadı. Id: {locationId}");

            var productsInLocation = _productRepository.GetProductsWithDetails()
                .Where(p => p.LocationId == locationId && p.Id != currentProductId)
                .ToList();

            // 1. Kapasite Kontrolü
            if (productsInLocation.Count >= location.Capacity)
            {
                throw new InvalidOperationException(
                    $"Bu raf ( {location.Name} ) dolu! Kapasite: {location.Capacity}. " +
                    $"Lütfen başka bir raf seçin.");
            }

            // 2. Kategori Uyumu Kontrolü (Aynı rafta farklı tür ürün olmasın)
            var otherCategory = productsInLocation.FirstOrDefault(p => p.Category != newCategory);
            if (otherCategory != null)
            {
                throw new InvalidOperationException(
                    $"Bu raf ( {location.Name} ) şu an '{otherCategory.Category}' kategorisindeki ürünler için ayrılmış. " +
                    $"'{newCategory}' kategorisindeki bir ürünü buraya koyamazsınız.");
            }
        }
        public void DeleteProduct(int id)
        {
            var product = GetProductById(id)
                ?? throw new KeyNotFoundException($"Ürün bulunamadı. Id: {id}");

            _productRepository.Delete(id);
        }

        public void StockIn(int productId, int quantity,
                            decimal unitPrice, string description)
        {
            var product = GetProductById(productId)
                ?? throw new KeyNotFoundException($"Ürün bulunamadı. Id: {productId}");

            product.AddStock(quantity);
            _productRepository.Update(product);

            var transaction = new Transaction(
                productId, product.Name,
                TransactionType.StokGirisi,
                quantity, unitPrice, description);

            _transactionRepository.Add(transaction);
            _alertSystem.NotifyAll(product);
        }
        //Services/ProductService.cs
        public void StockOut(int productId, int quantity, string description)
        {
            var product = GetProductById(productId)
                ?? throw new KeyNotFoundException($"Ürün bulunamadı. Id: {productId}");

            product.RemoveStock(quantity);
            _productRepository.Update(product);

            var transaction = new Transaction(
                productId, product.Name,
                TransactionType.StokCikisi,
                quantity, product.Price, description);

            transaction.CreatedAt = DateTime.Now;
            _transactionRepository.Add(transaction);

            // Observer Pattern devreye giriyor-Yayıncıyı tetikler
            _alertSystem.NotifyAll(product);
        }

        public Product? GetProductById(int id)
            => _productRepository.GetById(id);

        public Product? GetProductBySKU(string sku)
            => _productRepository.GetBySKU(sku);

        public IEnumerable<Product> GetAllProducts()
            => _productRepository.GetAllActive();

        public IEnumerable<Product> GetProductsWithDetails()
            => _productRepository.GetProductsWithDetails();

        public IEnumerable<Product> GetLowStockProducts()
            => _productRepository.GetLowStockProducts();

        public IEnumerable<Product> GetProductsByCategory(Category category)
            => _productRepository.GetByCategory(category);

        public IEnumerable<Product> SearchProducts(string keyword)
            => _productRepository.SearchByKeyword(keyword);

        public void DeleteTransaction(int transactionId)
        {
            var transaction = _transactionRepository.GetById(transactionId)
                ?? throw new KeyNotFoundException($"İşlem bulunamadı. Id: {transactionId}");

            // İade tipi veya 0 miktar stok değiştirmez
            if (transaction.Type != TransactionType.Iade && transaction.Quantity > 0)
            {
                var product = _productRepository.GetById(transaction.ProductId);
                if (product != null)
                {
                    if (transaction.Type == TransactionType.StokGirisi)
                    {
                        product.RemoveStock(transaction.Quantity);
                    }
                    else if (transaction.Type == TransactionType.StokCikisi)
                    {
                        product.AddStock(transaction.Quantity);
                    }
                    _productRepository.Update(product);
                    _alertSystem.NotifyAll(product);
                }
            }

            _transactionRepository.Delete(transactionId);
        }

        public void RegisterReturn(Return returnItem)
        {
            var product = GetProductById(returnItem.ProductId)
                ?? throw new KeyNotFoundException($"Ürün bulunamadı. Id: {returnItem.ProductId}");

            // İade kaydını ekle
            _returnRepository.Add(returnItem);

            string transactionNote = $"İade ({returnItem.ActionDisplay}): {returnItem.Reason} - {returnItem.Description}";

            // Eğer stoka geri alınacaksa stok güncelle
            if (returnItem.Action == ReturnAction.StokaGeriAl)
            {
                product.AddStock(returnItem.Quantity);
                _productRepository.Update(product);

                // Giriş işlemi olarak kaydet
                var transaction = new Transaction(
                    product.Id, product.Name,
                    TransactionType.StokGirisi,
                    returnItem.Quantity, product.Price,
                    transactionNote);
                
                _transactionRepository.Add(transaction);
            }
            else
            {
                // Hurdaya ayırma durumunda stok değişmez ama tarihçe için gerçek miktar kaydedilir
                var transaction = new Transaction(
                    product.Id, product.Name,
                    TransactionType.Iade, // Özel iade tipi
                    returnItem.Quantity, product.Price,
                    transactionNote);
                
                _transactionRepository.Add(transaction);
            }
            
            _alertSystem.NotifyAll(product);
        }

        public IEnumerable<Return> GetReturns()
        {
            return _returnRepository.GetAll()
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
    }
}