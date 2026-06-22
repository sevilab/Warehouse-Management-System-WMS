using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class SupplierRepository : BaseRepository<Supplier>
    {
        public SupplierRepository(AppDbContext context) : base(context) { }

        public override void Update(Supplier entity)
        {
            _context.SaveChanges();
        }

        public IEnumerable<Supplier> SearchByKeyword(string keyword)
        {
            keyword = keyword.ToLower();
            return _dbSet.Where(s => !s.IsDeleted &&
                  (s.Name.ToLower().Contains(keyword) ||
                   s.ContactPerson.ToLower().Contains(keyword))).ToList();
        }

        public IEnumerable<Supplier> GetActiveSuppliers()
        {
            return _dbSet.Where(s => !s.IsDeleted && s.IsActive).ToList();
        }
    }
}