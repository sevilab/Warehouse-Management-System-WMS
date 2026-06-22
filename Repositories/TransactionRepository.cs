using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public TransactionRepository(AppDbContext context) : base(context) { }


        // Neden? "50 adet tişört girdim" kaydını sonradan "10 adet girdim" yapabilsek 
        //  sistemin güvenilirliği biter.
        // Repositories/TransactionRepository.cs — YASAKLAR
        public override void Update(Transaction entity)
        {
            throw new InvalidOperationException("İşlem kayıtları güncellenemez.");
        }

        public IEnumerable<Transaction> GetByProduct(int productId)
        {
            return _dbSet.Where(t => t.ProductId == productId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt).ToList();
        }

        public IEnumerable<Transaction> GetByType(TransactionType type)
        {
            return _dbSet.Where(t => t.Type == type && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt).ToList();
        }

        public IEnumerable<Transaction> GetByDateRange(DateTime start, DateTime end)
        {
            return _dbSet.Where(t => t.CreatedAt >= start && t.CreatedAt <= end && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt).ToList();
        }

        public decimal GetTotalValueByType(TransactionType type)
        {
            return _dbSet.Where(t => t.Type == type).Sum(t => t.Quantity * t.UnitPrice);
        }
    }
}