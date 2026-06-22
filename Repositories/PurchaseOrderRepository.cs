using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Repositories
{
    public class PurchaseOrderRepository : BaseRepository<PurchaseOrder>
    {
        public PurchaseOrderRepository(AppDbContext context) : base(context) { }

        public override void Update(PurchaseOrder entity)
        {
            var existing = GetById(entity.Id);
            if (existing != null)
            {
                existing.Status = entity.Status;
                existing.DeliveryDate = entity.DeliveryDate;
                existing.Quantity = entity.Quantity;
                existing.UnitPrice = entity.UnitPrice;
                existing.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public IEnumerable<PurchaseOrder> GetPendingOrders()
        {
            return _dbSet.Where(o => o.Status == OrderStatus.Bekliyor && !o.IsDeleted)
                         .OrderByDescending(o => o.CreatedAt).ToList();
        }

        public IEnumerable<PurchaseOrder> GetOrdersByProduct(int productId)
        {
            return _dbSet.Where(o => o.ProductId == productId && !o.IsDeleted)
                         .OrderByDescending(o => o.CreatedAt).ToList();
        }
    }
}
