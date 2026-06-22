using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces
{
    // OOP: Abstraction — tedarikçi işlemlerinin sözleşmesi

    // Interfaces/ISupplierService.cs   
    public interface ISupplierService
    {
        void AddSupplier(Supplier supplier);
        void UpdateSupplier(int id, string name, string contactPerson,
                            string email, string phone, string address, bool isActive);
        void DeleteSupplier(int id);
        Supplier? GetSupplierById(int id);
        IEnumerable<Supplier> GetAllSuppliers();
        IEnumerable<Supplier> SearchSuppliers(string keyword);
        IEnumerable<Product> GetProductsBySupplier(int supplierId);
    }
}