using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Patterns
{
    public static class ProductFactory
    {
        public static Product CreateGiyim(string name, string sku,
            decimal price, int minStock, int supplierId,
            string unit = "Adet", string color = "", string size = "")
        {
            return new Product(name, sku, price, minStock,
                Category.Üst_Giyim, supplierId, null, unit, "", color, size);
        }
        public static Product CreateDisGiyim(string name, string sku,
            decimal price, int minStock, int supplierId, string color = "", string size = "")
        {
            return new Product(name, sku, price, minStock,
                Category.Dış_Giyim, supplierId, null, "Adet", "", color, size);
        }
        public static Product CreateSporGiyim(string name, string sku,
            decimal price, int minStock, int supplierId, string color = "", string size = "")
        {
            return new Product(name, sku, price, minStock,
                Category.Spor_Giyim, supplierId, null, "Adet", "", color, size);
        }
        public static Product CreateGenel(string name, string sku,
            decimal price, int minStock, Category category,
            int supplierId, string unit, string color = "", string size = "")
        {
            return new Product(name, sku, price, minStock,
                category, supplierId, null, unit, "", color, size);
        }
    }
}