using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagementSystem.Models
{
    public class Transaction : BaseEntity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = "Sistem";

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;

        [NotMapped]
        public string TypeDisplay => Type switch
        {
            TransactionType.StokGirisi => "Giriş",
            TransactionType.StokCikisi => "Çıkış",
            TransactionType.Iade => "İade",
            _ => "Bilinmiyor"
        };

        [NotMapped]
        public string TypeColor => Type switch
        {
            TransactionType.StokGirisi => "#2E7D32", // Green
            TransactionType.StokCikisi => "#C62828", // Red
            TransactionType.Iade => Description.Contains("Hurda") || Description.Contains("İmha") ? "#E65100" : "#4527A0", // Orange for scrap, Purple for back-to-stock info
            _ => "#757575"
        };

        [NotMapped]
        public string TypeBackground => Type switch
        {
            TransactionType.StokGirisi => "#E8F5E9",
            TransactionType.StokCikisi => "#FFEBEE",
            TransactionType.Iade => Description.Contains("Hurda") || Description.Contains("İmha") ? "#FFF3E0" : "#F3E5F5",
            _ => "#F5F5F5"
        };

        // EF Core için parametresiz constructor
        public Transaction() { }

        public Transaction(int productId, string productName,
                           TransactionType type, int quantity,
                           decimal unitPrice, string description,
                           string performedBy = "Sistem")
        {
            ProductId = productId;
            ProductName = productName;
            Type = type;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Description = description;
            PerformedBy = performedBy;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            string typeStr = Type switch
            {
                TransactionType.StokGirisi => "📥 Giriş",
                TransactionType.StokCikisi => "📤 Çıkış",
                TransactionType.Transfer => "🔄 Transfer",
                TransactionType.Iade => "↩️ İade",
                _ => "Bilinmiyor"
            };
            return $"{typeStr} | {ProductName} | Miktar: {Quantity} | " +
                   $"Toplam: {TotalPrice:C2} | Tarih: {CreatedAt:dd.MM.yyyy HH:mm}";
        }
    }
}