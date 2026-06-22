using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WarehouseManagementSystem.Models

{    // TÜREYEN SINIF
    public class Product : BaseEntity
    {   
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Category Category { get; set; }
        public int MinStockLevel { get; set; }
        public int SupplierId { get; set; }
        public string Unit { get; set; } = "Adet";

        // Teknik Kapsülleme: Herkes OKUYABİLİR ama dışarıdan SET EDEMEZ
        public int StockQuantity { get; private set; }
        public decimal Price { get; private set; }
        public string Color { get; set; } = string.Empty; // Added
        public string Size { get; set; } = string.Empty;  // Added
        public int? LocationId { get; set; }
        public virtual Location? Location { get; set; }

        [NotMapped]
        public string SupplierName { get; set; } = string.Empty;

        [NotMapped]
        public string LocationName { get; set; } = string.Empty;

        [NotMapped]
        public AlertLevel AlertLevel
        {
            get
            {
                if (StockQuantity == 0) return AlertLevel.Kritik;
                if (StockQuantity <= MinStockLevel) return AlertLevel.Düşük;
                return AlertLevel.Normal;
            }
        }

        // EF Core için parametresiz constructor
        public Product() { }

        public Product(string name, string sku, decimal price,
                       int minStockLevel, Category category,
                       int supplierId, int? locationId = null, 
                       string unit = "Adet", string description = "",
                       string color = "", string size = "")
        {
            Name = name;
            SKU = sku;
            Price = price;
            MinStockLevel = minStockLevel;
            Category = category;
            SupplierId = supplierId;
            LocationId = locationId;
            Unit = unit;
            Description = description;
            Color = color;
            Size = size;
            StockQuantity = 0;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        // Stok eklemek için kontrollü kapı
        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Miktar sıfırdan büyük olmalıdır.");
            StockQuantity += quantity;
            UpdatedAt = DateTime.Now;
        }
        // Stok çıkarmak için kontrollü kapı — Yetersiz stok kontrolü burada yapılır
        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Miktar sıfırdan büyük olmalıdır.");
            if (quantity > StockQuantity)
                throw new InvalidOperationException("Yetersiz stok.");
            StockQuantity -= quantity;
            UpdatedAt = DateTime.Now;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Fiyat negatif olamaz.");
            Price = newPrice;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateInfo(string name, string description, int minStockLevel, string color, string size)
        {
            Name = name;
            Description = description;
            MinStockLevel = minStockLevel;
            Color = color;
            Size = size;
            UpdatedAt = DateTime.Now;
        }

        public override string ToString() // Models/Product.cs — ürüne özel versiyon
        {
            return $"{Name} | SKU: {SKU} | Stok: {StockQuantity} {Unit} | Fiyat: {Price:C2}";
        }
    }
}