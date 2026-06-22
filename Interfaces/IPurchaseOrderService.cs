using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces
{
    public interface IPurchaseOrderService
    {
        void CreateOrder(int productId, int quantity, int supplierId, decimal unitPrice);
        void MarkAsReceived(int orderId);
        IEnumerable<PurchaseOrder> GetAllOrders();
        IEnumerable<PurchaseOrder> GetPendingOrders();
        void DeleteOrder(int orderId);
    }
}
