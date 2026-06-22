using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces
{
    // OOP: Abstraction — ürün işlemlerinin sözleşmesi

    //Interfaces/IProductService.cs
    public interface IProductService
    {
        void AddProduct(Product product);
        void UpdateProduct(int id, string name, string description,
                    int minStockLevel, decimal price, Category category, int? locationId,
                    string color, string size);
        void DeleteProduct(int id);
        void StockIn(int productId, int quantity, decimal unitPrice, string description);
        void StockOut(int productId, int quantity, string description);
        Product? GetProductById(int id);
        Product? GetProductBySKU(string sku);
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsWithDetails(); // Added
        IEnumerable<Product> GetLowStockProducts();
        IEnumerable<Product> GetProductsByCategory(Category category);
        IEnumerable<Product> SearchProducts(string keyword);
        void DeleteTransaction(int transactionId);
        void RegisterReturn(Return returnItem);
        IEnumerable<Return> GetReturns();
    }
}