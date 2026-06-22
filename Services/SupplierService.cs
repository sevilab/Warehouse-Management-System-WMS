using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.Services
{
    // OOP: Encapsulation + Abstraction
    public class SupplierService : ISupplierService
    {
        private readonly SupplierRepository _supplierRepository;
        private readonly ProductRepository _productRepository;

        public SupplierService(SupplierRepository supplierRepository,
                               ProductRepository productRepository)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
        }

        public void AddSupplier(Supplier supplier)
        {
            _supplierRepository.Add(supplier);
        }

        public void UpdateSupplier(int id, string name, string contactPerson,
                                   string email, string phone, string address, bool isActive)
        {
            var supplier = GetSupplierById(id)
                ?? throw new KeyNotFoundException($"Tedarikçi bulunamadı. Id: {id}");

            supplier.UpdateInfo(name, contactPerson, email, phone, address);
            if (isActive) supplier.Activate();
            else supplier.Deactivate();
            
            _supplierRepository.Update(supplier);
        }

        public void DeleteSupplier(int id)
        {
            var supplier = GetSupplierById(id)
                ?? throw new KeyNotFoundException($"Tedarikçi bulunamadı. Id: {id}");

            _supplierRepository.Delete(id);
        }

        public Supplier? GetSupplierById(int id)
            => _supplierRepository.GetById(id);

        public IEnumerable<Supplier> GetAllSuppliers()
            => _supplierRepository.GetAllActive();

        public IEnumerable<Supplier> SearchSuppliers(string keyword)
            => _supplierRepository.SearchByKeyword(keyword);

        public IEnumerable<Product> GetProductsBySupplier(int supplierId)
            => _productRepository.GetBySupplier(supplierId);
    }
}