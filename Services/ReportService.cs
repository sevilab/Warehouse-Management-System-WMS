using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;
using System.Text;

namespace WarehouseManagementSystem.Services
{
    public class ReportService
    {
        private readonly ProductRepository _productRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly TransactionRepository _transactionRepository;

        public ReportService(ProductRepository productRepository,
                             SupplierRepository supplierRepository,
                             TransactionRepository transactionRepository)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _transactionRepository = transactionRepository;
        }

        public StockSummary GetStockSummary()
        {
            var products = _productRepository.GetAllActive().ToList();

            return new StockSummary
            {
                TotalProducts = products.Count,
                TotalStockValue = products.Sum(p => p.StockQuantity * p.Price),
                LowStockCount = products.Count(p => p.AlertLevel == AlertLevel.Düşük),
                CriticalStockCount = products.Count(p => p.AlertLevel == AlertLevel.Kritik),
                OutOfStockCount = products.Count(p => p.StockQuantity == 0),
                TotalSuppliers = _supplierRepository.GetAllActive().Count(),
                TotalTransactions = _transactionRepository.GetAll().Count()
            };
        }

        public IEnumerable<CategorySummary> GetCategorySummary()
        {
            var products = _productRepository.GetAllActive().ToList();

            return products
                .GroupBy(p => p.Category)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    ProductCount = g.Count(),
                    TotalValue = g.Sum(p => p.StockQuantity * p.Price)
                })
                .OrderByDescending(c => c.TotalValue)
                .ToList();
        }

        public IEnumerable<Transaction> GetRecentTransactions(int count = 10)
        {
            return _transactionRepository.GetAll()
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToList();
        }

        public IEnumerable<Transaction> GetTransactionsByDateRange(
            DateTime start, DateTime end)
        {
            return _transactionRepository.GetByDateRange(start, end);
        }

        public IEnumerable<DailyTransactionSummary> GetDailyTransactionTrend(int days = 7)
        {
            var end = DateTime.Now.Date;
            var start = end.AddDays(-days + 1);
            
            var transactions = _transactionRepository.GetByDateRange(start, end.AddDays(1).AddTicks(-1)).ToList();
            
            return Enumerable.Range(0, days)
                .Select(i => start.AddDays(i))
                .Select(date => new DailyTransactionSummary
                {
                    Date = date,
                    EntryCount = transactions.Count(t => t.CreatedAt.Date == date && t.Type == TransactionType.StokGirisi),
                    ExitCount = transactions.Count(t => t.CreatedAt.Date == date && t.Type == TransactionType.StokCikisi)
                })
                .ToList();
        }
        // Services/ReportService.cs
        public string GetProductsCsv()// Strateji 1: Ürün listesini CSV olarak ver
        {
            var products = _productRepository.GetAllActive().ToList();
            var csv = new StringBuilder();
            csv.AppendLine("ID;Ürün Adı;SKU;Kategori;Miktar;Birim Fiyat;Toplam Değer;Tedarikçi;Durum");

            foreach (var p in products)
            {
                csv.AppendLine($"{p.Id};{p.Name};{p.SKU};{p.Category};{p.StockQuantity};{p.Price:F2};{p.StockQuantity * p.Price:F2};{p.SupplierName};{p.AlertLevel}");
            }
            return csv.ToString();
        }
        // Strateji 2: İşlem geçmişini CSV olarak ver
        public string GetTransactionsCsv()
        {
            var transactions = _transactionRepository.GetAll().OrderByDescending(t => t.CreatedAt).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("ID;Tarih;Ürün;İşlem Tipi;Miktar;Birim Fiyat;Toplam Tutar;Not");

            foreach (var t in transactions)
            {
                string typeDesc = t.Type switch
                {
                    TransactionType.StokGirisi => "Giriş",
                    TransactionType.StokCikisi => "Çıkış",
                    TransactionType.Iade => "İade",
                    _ => "Bilinmiyor"
                };
                csv.AppendLine($"{t.Id};{t.CreatedAt:dd.MM.yyyy HH:mm};{t.ProductName};{typeDesc};{t.Quantity};{t.UnitPrice:F2};{t.TotalPrice:F2};{t.Description}");
            }
            return csv.ToString();
        }
        // Strateji 3: Tedarikçi listesini CSV olarak ver
        public string GetSuppliersCsv()
        {
            var suppliers = _supplierRepository.GetAll().ToList();
            var csv = new StringBuilder();
            csv.AppendLine("ID;Tedarikçi Adı;Yetkili;Telefon;E-posta;Adres;Oluşturma Tarihi");

            foreach (var s in suppliers)
            {
                csv.AppendLine($"{s.Id};{s.Name};{s.ContactPerson};{s.Phone};{s.Email};{s.Address};{s.CreatedAt:dd.MM.yyyy}");
            }
            return csv.ToString();
        }
    }

    public class DailyTransactionSummary
    {
        public DateTime Date { get; set; }
        public int EntryCount { get; set; }
        public int ExitCount { get; set; }
    }

    public class StockSummary
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockCount { get; set; }
        public int CriticalStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class CategorySummary
    {
        public Category Category { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
    }
}