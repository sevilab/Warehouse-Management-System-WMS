using System;

namespace WarehouseManagementSystem.Models
{
    public class Return : BaseEntity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public ReturnReason Reason { get; set; }
        public ReturnAction Action { get; set; }
        public string Description { get; set; } = string.Empty;
        
        // UI Helpers
        public string ReasonDisplay => Reason.ToString();
        public string ActionDisplay => Action == ReturnAction.StokaGeriAl ? "Stoka Geri Alındı" : "Hurdaya Ayrıldı";
    }
}
