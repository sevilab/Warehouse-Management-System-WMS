using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces
{
    // Observer Pattern için temel sözleşme
    // OOP: Abstraction

    // Interfaces/INotificationService.cs
    // Bildirim YAYINLAMA arayüzü — sadece "abone ekle/çıkar, herkese haber ver" işlemleri
    public interface INotificationService
    {
        void Subscribe(IStockObserver observer);
        void Unsubscribe(IStockObserver observer);
        void NotifyAll(Product product);
    }
    // Uyarıyı ALMA arayüzü — sadece "uyarı geldiğinde ne yapacağım" işlemi
    public interface IStockObserver
    {
        string ObserverName { get; }
        void OnStockAlert(Product product, AlertLevel alertLevel);
    }
}