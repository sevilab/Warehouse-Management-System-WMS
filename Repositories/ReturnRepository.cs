using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class ReturnRepository : BaseRepository<Return>
    {
        public ReturnRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Return> GetByProductId(int productId)
        {
            return _dbSet.Where(r => r.ProductId == productId).ToList();
        }

        public override void Update(Return entity)
        {
            _context.SaveChanges();
        }
    }
}
