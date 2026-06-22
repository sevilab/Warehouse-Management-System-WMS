using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Patterns
{
    // Observer Pattern — stok değişince tüm aboneleri otomatik bilgilendirir
    // Design Pattern: Observer ✅
    public class StockAlertSystem : INotificationService
    {// Patterns/StockAlertSystem.cs /abone listesi
        private readonly List<IStockObserver> _observers = new List<IStockObserver>();
        private static StockAlertSystem? _instance;
        public static StockAlertSystem Instance =>
            _instance ??= new StockAlertSystem();

        private StockAlertSystem() { } // Dışarıdan new yapmak mümkün değil
        public void Subscribe(IStockObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Unsubscribe(IStockObserver observer)
        {
            _observers.Remove(observer);
        }
        //Subject(Yayıncı Sınıf,Haber veren)
        // Herkese bildir
        public void NotifyAll(Product product)
        {
            if (product.AlertLevel == AlertLevel.Normal) return;

            foreach (var observer in _observers)
            {
                observer.OnStockAlert(product, product.AlertLevel);                 // Her biri kendi tepkisini verir
            }
        }

        public int ObserverCount => _observers.Count;
    }

    // Concrete Observer — konsol/log bildirimi
    // OOP: Polymorphism — IStockObserver'ı farklı şekilde implemente eder
    //Patterns/StockAlertSystem.cs
    //  Konsola ve listeye log yazar
    public class LogObserver : IStockObserver
    {
        public string ObserverName => "Log Gözlemcisi";

        public List<string> Logs { get; } = new List<string>();
        public void OnStockAlert(Product product, AlertLevel alertLevel)
        {
            string message = alertLevel switch
            {
                AlertLevel.Kritik => $"🔴 KRİTİK: '{product.Name}' ürününün stoğu tükendi!",
                AlertLevel.Düşük => $"🟡 UYARI: '{product.Name}' ürününün stoğu azaldı! " +
                                    $"Mevcut: {product.StockQuantity}",
                _ => string.Empty};
            if (!string.IsNullOrEmpty(message))
            {
                var log = $"[{DateTime.Now:dd.MM.yyyy HH:mm}] {message}";
                Logs.Add(log);
            }// Sessizce listeye ekler, kullanıcı görmez
        }
    }
    //  Ekranda bir uyarı kutusu açar (kullanıcıyı doğrudan bilgilendirir)
    public class UIAlertObserver : IStockObserver
    {   public string ObserverName => "UI Uyarı Gözlemcisi";

        public event Action<string, AlertLevel>? AlertRaised;

        public void OnStockAlert(Product product, AlertLevel alertLevel)
        {
            string message = alertLevel switch
            {
                AlertLevel.Kritik => $"'{product.Name}' stoğu tükendi!",
                AlertLevel.Düşük => $"'{product.Name}' stoğu kritik seviyede! " +
                                    $"Kalan: {product.StockQuantity}",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(message))
                AlertRaised?.Invoke(message, alertLevel);
        }
    }
}