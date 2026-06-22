namespace WarehouseManagementSystem.Models
{
    public enum OrderStatus
    {
        Bekliyor,    // Pending
        TeslimAlindi // Received
    }

    public class PurchaseOrder : BaseEntity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Bekliyor;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate { get; set; }

        public PurchaseOrder() { }

        public PurchaseOrder(int productId, string productName, int supplierId, string supplierName, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            SupplierId = supplierId;
            SupplierName = supplierName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
