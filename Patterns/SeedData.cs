using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Patterns
{
    public static class SeedData
    {
        public static void Load(ProductService productService,
                                SupplierService supplierService,
                                LocationService locationService)
        {
            // 1. Lokasyonlar (Hafızada kontrol ile optimize et)
            var existingLocNames = new HashSet<string>(locationService.GetAllLocations().Select(l => l.Name));
            
            string[] rows = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N" };
            string[] zones = { "Ön", "Orta", "Arka" };
            int[] floors = { 1, 2, 3 };

            foreach (var floor in floors)
            {
                foreach (var zone in zones)
                {
                    foreach (var row in rows)
                    {
                        // 101-110 arası (veya 201-210, 301-310)
                        for (int i = 1; i <= 10; i++)
                        {
                            var num = (floor * 100) + i;
                            var locName = $"{row}-{num}";
                            if (!existingLocNames.Contains(locName))
                            {
                                locationService.AddLocation(new Location(locName, zone, floor, $"{zone} Bölge, {floor}. Kat, {row} Sırası"));
                                existingLocNames.Add(locName);
                            }
                        }
                    }
                }
            }

            var allLocs = locationService.GetAllLocations().ToList();

            // 2. Tedarikçiler (Hafızada kontrol)
            var allSuppliers = supplierService.GetAllSuppliers().ToList();
            
            Supplier GetOrAddSupplier(string name, string contact, string email, string phone, string city)
            {
                var existing = allSuppliers.FirstOrDefault(s => s.Name == name);
                if (existing != null) return existing;

                var newS = new Supplier(name, contact, email, phone, city);
                supplierService.AddSupplier(newS);
                allSuppliers.Add(newS);
                return newS;
            }

            var s1 = GetOrAddSupplier("TechStyle A.Ş.", "Ahmet Yılmaz", "ahmet@techstyle.com", "05321234567", "İstanbul");
            var s2 = GetOrAddSupplier("ModaTextil Ltd.", "Ayşe Kaya", "ayse@modatextil.com", "05334567890", "İstanbul");
            var s3 = GetOrAddSupplier("TrendHouse", "Mehmet Demir", "mehmet@trendhouse.com", "05351234567", "İzmir");

            // 3. Ürünler (Hafızada kontrol)
            var existingSkus = new HashSet<string>(productService.GetAllProducts().Select(p => p.SKU));

            void AddProductWithStock(Product p, int initialStock)
            {
                if (!existingSkus.Contains(p.SKU))
                {
                    productService.AddProduct(p);
                    existingSkus.Add(p.SKU);
                    if (initialStock > 0)
                    {
                        productService.StockIn(p.Id, initialStock, p.Price, "Başlangıç");
                    }
                }
            }

            var p1 = ProductFactory.CreateGiyim("Erkek Slim Fit Jean", "JN-001", 599, 15, s1.Id, "Adet", "Mavi", "32/34");
            p1.LocationId = allLocs.FirstOrDefault(l => l.Name == "A-101")?.Id;
            AddProductWithStock(p1, 55);

            var p2 = ProductFactory.CreateDisGiyim("Kadın Trençkot", "TK-001", 1299, 8, s1.Id, "Bej", "M");
            p2.LocationId = allLocs.FirstOrDefault(l => l.Name == "B-102")?.Id;
            AddProductWithStock(p2, 5); // Düşük stok

            var p3 = ProductFactory.CreateGiyim("Erkek Polo Tişört", "TS-001", 299, 20, s2.Id, "Adet", "Beyaz", "L");
            p3.LocationId = allLocs.FirstOrDefault(l => l.Name == "C-201")?.Id;
            AddProductWithStock(p3, 100);

            var p4 = ProductFactory.CreateGiyim("Kadın Elbise", "EL-001", 899, 10, s2.Id, "Adet", "Siyah", "S");
            p4.LocationId = allLocs.FirstOrDefault(l => l.Name == "D-202")?.Id;
            AddProductWithStock(p4, 40);

            var p5 = ProductFactory.CreateSporGiyim("Kapüşonlu Sweatshirt", "SW-001", 499, 25, s2.Id, "Gri", "XL");
            p5.LocationId = allLocs.FirstOrDefault(l => l.Name == "A-301")?.Id;
            AddProductWithStock(p5, 80);

            var p6 = ProductFactory.CreateGiyim("Erkek Takım Elbise", "TE-001", 3499, 5, s3.Id, "Adet", "Lacivert", "52/6");
            p6.LocationId = allLocs.FirstOrDefault(l => l.Name == "B-302")?.Id;
            AddProductWithStock(p6, 3); // Düşük stok

            var p7 = ProductFactory.CreateDisGiyim("Kadın Blazer Ceket", "BJ-001", 1599, 8, s3.Id, "Kırmızı", "38");
            p7.LocationId = allLocs.FirstOrDefault(l => l.Name == "A-202")?.Id;
            AddProductWithStock(p7, 25);

            var p8 = ProductFactory.CreateGenel("Deri Kemer", "KM-001", 199, 30, Category.Aksesuar, s1.Id, "Adet", "Siyah", "Standart");
            p8.LocationId = allLocs.FirstOrDefault(l => l.Name == "B-201")?.Id;
            AddProductWithStock(p8, 150);
        }
    }
}