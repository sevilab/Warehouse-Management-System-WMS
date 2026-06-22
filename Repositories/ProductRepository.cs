using Microsoft.EntityFrameworkCore; // Added for Include
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{    //TÜREYEN SINIF
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Product> GetProductsWithDetails()
        {
            return _dbSet
                .Include(p => p.Location) 
                .Where(p => !p.IsDeleted)
                .ToList();
        }
        // Repositories/ProductRepository.cs
        public override void Update(Product entity)
        {
            _context.SaveChanges();
        }
        public IEnumerable<Product> GetByCategory(Category category)
        {
            return _dbSet.Where(p => !p.IsDeleted && p.Category == category).ToList();
        }
        public IEnumerable<Product> GetLowStockProducts()
        {
            return _dbSet.Where(p => !p.IsDeleted &&
                   p.StockQuantity <= p.MinStockLevel).ToList();
        }
        public IEnumerable<Product> SearchByKeyword(string keyword)
        {
            keyword = keyword.ToLower();
            return _dbSet.Where(p => !p.IsDeleted &&
                  (p.Name.ToLower().Contains(keyword) ||
                   p.SKU.ToLower().Contains(keyword))).ToList();
        }
        public Product? GetBySKU(string sku)
        {
            return _dbSet.FirstOrDefault(p => !p.IsDeleted &&
                   p.SKU.ToLower() == sku.ToLower());
        }

        public IEnumerable<Product> GetBySupplier(int supplierId)
        {
            return _dbSet.Where(p => !p.IsDeleted &&
                   p.SupplierId == supplierId).ToList();
        }
    }
}