using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly PurchaseOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;

        public PurchaseOrderService(PurchaseOrderRepository orderRepository, 
                                     IProductService productService,
                                     ISupplierService supplierService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _supplierService = supplierService;
        }

        public void CreateOrder(int productId, int quantity, int supplierId, decimal unitPrice)
        {
            var product = _productService.GetProductById(productId)
                ?? throw new KeyNotFoundException("Ürün bulunamadı.");

            var supplier = _supplierService.GetSupplierById(supplierId)
                ?? throw new KeyNotFoundException("Tedarikçi bulunamadı.");

            var order = new PurchaseOrder(
                productId, product.Name, 
                supplier.Id, supplier.Name, 
                quantity, unitPrice);

            _orderRepository.Add(order);
        }

        public void MarkAsReceived(int orderId)
        {
            var order = _orderRepository.GetById(orderId)
                ?? throw new KeyNotFoundException("Sipariş bulunamadı.");

            if (order.Status == OrderStatus.TeslimAlindi)
                return;

            order.Status = OrderStatus.TeslimAlindi;
            order.DeliveryDate = DateTime.Now;
            _orderRepository.Update(order);

            // Stok güncelleme (productService.StockIn otomatik transaction loglar)
            _productService.StockIn(order.ProductId, order.Quantity, order.UnitPrice, $"Satın Alma Siparişi #{order.Id} Teslimatı");
        }

        public IEnumerable<PurchaseOrder> GetAllOrders()
            => _orderRepository.GetAllActive();

        public IEnumerable<PurchaseOrder> GetPendingOrders()
            => _orderRepository.GetPendingOrders();

        public void DeleteOrder(int orderId)
            => _orderRepository.Delete(orderId);
    }
}
