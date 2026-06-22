using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Repositories;
using System.Data.Common;

namespace WarehouseManagementSystem.Patterns
{//Patterns/WarehouseManager.cs
    public sealed class WarehouseManager
    {
        private static WarehouseManager? _instance;
        private static readonly object _lock = new object();
        public AppDbContext DbContext { get; private set; }
        public ProductRepository Products { get; private set; }
        public SupplierRepository Suppliers { get; private set; }
        public TransactionRepository Transactions { get; private set; }
        public UserRepository Users { get; private set; }
        public LocationRepository Locations { get; private set; }
        public ReturnRepository Returns { get; private set; }
        public PurchaseOrderRepository PurchaseOrders { get; private set; }
        private WarehouseManager()
        { // Dışarıdan 'new' ile sınıf oluşturulamaz (private constructor)
            DbContext = new AppDbContext();
            DbContext.Database.EnsureCreated();
            Products = new ProductRepository(DbContext);
            Suppliers = new SupplierRepository(DbContext);
            Transactions = new TransactionRepository(DbContext);
            Users = new UserRepository(DbContext);
            Locations = new LocationRepository(DbContext);
            Returns = new ReturnRepository(DbContext);
            PurchaseOrders = new PurchaseOrderRepository(DbContext);
        }
        public static WarehouseManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new WarehouseManager();
                        using (DbConnection connection = _instance.DbContext.Database.GetDbConnection())
                        {
                            connection.Open();
                            using (DbCommand command = connection.CreateCommand())
                            {
                                command.CommandText = "UPDATE Locations SET Floor = Floor / 100 WHERE Floor >= 100;";
                                try { command.ExecuteNonQuery(); } catch { }
                            }
                            connection.Close();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}