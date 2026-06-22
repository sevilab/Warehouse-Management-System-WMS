# Warehouse-Management-System-WMS
A modern Desktop Application developed in C# and WPF implementing Advanced OOP, SOLID, and Enterprise Design Patterns with EF Core and SQLite.
Warehouse Management System (Depo Yönetim Sistemi)
# Warehouse Management System (WMS) - Enterprise Desktop Application 📦

This project is an industrial-scale **Warehouse Management System (WMS)** simulation developed using **C#** and **WPF (Windows Presentation Foundation)** technologies, fully aligned with modern software architecture standards. The primary objective of this project is not merely to provide a functional user interface, but to comprehensively implement **Advanced Object-Oriented Programming (OOP) Principles**, **SOLID Software Design Principles**, and **Enterprise Design Patterns** within a live, database-driven operational scenario.

The application leverages the **Entity Framework Core (EF Core)** ORM framework and **SQLite**, a lightweight, portable relational database engine, for its data persistence layer. This ensures a stable, "plug-and-play" (portable) architecture without requiring any external server configurations or installations.

---

## 🏗️ Architectural Structure: MVVM (Model-View-ViewModel)

The project strictly adheres to the **MVVM** design pattern, which decouples user interface (UI) components from the core business logic, ensuring high testability, maintainability, and clean separation of concerns:

* **Model (Data Layer):** Represents the database schemas, entities, and domain business rules. It maps directly to the database via `AppDbContext`.
* **View (Presentation Layer):** Comprises the `WPF/XAML` user interfaces handling user interactions. It contains zero business logic and is entirely driven by declarative Data Binding.
* **ViewModel (Bridge Layer):** Establishes a two-way communication channel between the View and the Model. It captures UI events, delegates them to business services, and dynamically updates the presentation layer.

---

## 📋 Enterprise Modules & Core Features

The system simulates real-world supply chain and warehouse workflows through 9 fully integrated core modules:

1.  **Dashboard & Analytics:** A centralized command screen displaying real-time metrics, including total product counts, category distributions, total financial inventory values, and instant counts of items falling below critical safety thresholds.
2.  **Product & Inventory Management:** Full-scale CRUD (Create, Read, Update, Delete) workflows featuring SKU (Stock Keeping Unit) tracking, dynamic category assignments, multi-variant attributes (e.g., size, color), and physical warehouse location tagging.
3.  **Supplier Chain Management:** A dedicated registry for active vendor entities, company records, corporate contact persons, and supplier-specific product procurement histories.
4.  **Purchase Orders (PO):** Digital creation and financial tracking of procurement requests sent to suppliers, managing order lifecycle statuses smoothly from "Pending" to "Fulfilled".
5.  **Secure Stock In/Out Pipelines:** A bulletproof stock movement workflow that enforces unit price recording, chronological timestamps, and mandatory transaction reasons (e.g., Inflow, Sale, Scrap) for every inventory mutation.
6.  **Transaction Ledger (Audit Trail):** An immutable, chronologically ordered transaction log stored securely in the database to satisfy industrial auditing standards and historical tracking requirements.
7.  **Returns & Scrap (Quality Control):** A dynamic quality inspection mechanism that evaluates defective or returned items to decide whether they should be re-allocated into active inventory or written off and transferred to the designated scrap yard.
8.  **Physical Location Management:** A digital twin mapping warehouse spatial economics. It segments inventory across specific Floors, Sections, and Shelf Numbers, reducing item retrieval times to zero.
9.  **User Authentication & Privilege Controls:** A secure Login/Logout environment hosting discrete `Admin` (Full Privileges) and `Standard User` (Restricted Access) security roles. Unauthorized personnel are strictly barred from critical actions and destructive delete operations at the architectural level.

---

## 💎 Applied Software Engineering & Clean Code Principles

To elevate code quality to enterprise-grade standards, this codebase rigidly executes **SOLID** software principles and **Advanced OOP** design paradigms.

### 🧩 Advanced OOP Implementation
* **Encapsulation:** State parameters within models and services are strictly protected from external mutation. For instance, the `StockQuantity` property in `Product.cs` cannot be edited directly by external layers; it is modified exclusively through concrete domain methods like `StockIn()` and `StockOut()`.
* **Inheritance:** Code duplication is eliminated by centralizing boilerplate attributes in base classes. Shared fields like `Id`, `CreatedDate`, and `IsDeleted` are inherited from `BaseEntity`; common WPF properties derive from `BaseViewModel`; and unified data pipeline mechanisms stem from `BaseRepository`.
* **Abstraction & Interface Segregation:** The UI and ViewModel layers never communicate directly with raw database concrete classes. Communications pass entirely through highly-focused, minimal interfaces (`IRepository<T>`, `IProductService`, `ISupplierService`), rendering the system architecture beautifully decoupled and loosely bound.

---

## 🧩 Applied Design Patterns

To guarantee scalability, extensibility, and modularity, **7 Design Patterns** across 3 distinct structural categories are actively woven into the architecture:

### 1. Creational Patterns
* **Singleton Pattern (`WarehouseManager`):** Guarantees that a single persistent database connection context instance (`DbContext`) is maintained throughout the application lifecycle. This minimizes memory overhead and ensures cross-thread safety.
* **Factory Method Pattern (`ProductFactory`):** Encapsulates and standardizes complex multi-attribute instantiation routines and default state assignments for diverse product types (e.g., Apparel, Stationery, Textiles) within a unified factory interface.

### 2. Structural Patterns
* **Repository Pattern (`BaseRepository`):** Fully abstracts raw Entity Framework Core SQL/LINQ commands away from the business layer. If the infrastructure migrates from SQLite to SQL Server or PostgreSQL, only the repository implementation needs an update, leaving the higher layers completely untouched.
* **Facade Pattern (`WarehouseManager`):** Wraps numerous complex internal database sub-systems (ProductRepository, SupplierRepository, TransactionRepository, etc.) behind a clean, single-entry structural facade. ViewModels interact with this single unified coordinator instead of handling a web of dependencies.
* **Decorator / Converter Pattern (`*Converter.cs`):** Leverages WPF’s structural `IValueConverter` pattern to dynamically alter the presentation of raw database primitives (e.g., boolean flags or numerical floor coordinates) into context-aware visual elements (color-coded highlights, string labels) without mutating the underlying data source.

### 3. Behavioral Patterns
* **Observer Pattern (`StockAlertSystem`):** Operates as an event-driven broadcast network. The moment any inventory metric breaches its safety threshold, all registered subscribers—such as the `LogObserver` (writes background audit txt/DB logs) and the `UIAlertObserver` (triggers red UI warning prompts)—are instantly executed.
* **Command Pattern (`RelayCommand`):** Maps UI button triggers and user interface interactions directly to backend service routines. By implementing robust "Guard Conditions," it handles automated UI states, disabling actionable buttons if field prerequisites are unfulfilled or user permissions are inadequate.
* **Strategy Pattern (`ReportService`):** Abstracts data extraction and reporting algorithms into interchangeable strategies. This allows the system to seamlessly switch between different reporting formats (e.g., CSV generation, Excel extraction, or plain Text documentation) at runtime.
* **Template Method Pattern (`BaseRepository.Update`):** Defines the behavioral skeleton of the database update operation within the abstract parent class, while allowing concrete child repositories (like `ProductRepository`) to override and plug in their unique entity-specific mutation steps.

---

## 🛠️ Technical Stack & Dependencies

* **Language & Framework:** C# / .NET 10.0 / WPF (XAML)
* **ORM & Database:** Entity Framework Core (EF Core) / SQLite (The relational database initializes automatically on the first launch; no standalone server setup required).
* **UI Themes:** Advanced WPF Resource Dictionaries and Modern Material Design Component Libraries.

---

## 🚀 Installation & Execution Guidelines

1.  Clone or download this repository to your local machine.
2.  Open your Visual Studio IDE and load the next-generation solution blueprint by double-clicking the `WarehouseManagementSystem.slnx` file in the root folder.
3.  Build the solution (`Build Solution` or `Ctrl+Shift+B`) to trigger NuGet to automatically restore all essential packages (**Microsoft.EntityFrameworkCore.Sqlite**, **MaterialDesignThemes**).
4.  Press `F5` to execute the application locally. On its inaugural execution, the backend pipeline will automatically generate the localized SQLite database file within the project's build binary directory.

************************
# Warehouse Management System (WMS) - Kurumsal Depo Yönetim Sistemi 📦

Bu proje; modern yazılım mimarisi standartlarına uygun olarak **C#** ve **WPF (Windows Presentation Foundation)** teknolojileri kullanılarak geliştirilmiş, endüstriyel ölçekte bir **Depo Yönetim Sistemi (WMS)** simülasyonudur. Projenin birincil amacı sadece işlevsel bir arayüz sunmak değil; **Nesne Yönelimli Programlama (OOP) İlkelerini**, **SOLID Yazılım Prensiplerini** ve **Kurumsal Tasarım Desenlerini (Enterprise Design Patterns)** canlı, veri tabanı bağlantılı bir senaryo üzerinde eksiksiz olarak hayata geçirmektir.

Uygulama, veri tabanı katmanında **Entity Framework Core (EF Core)** ORM aracını ve hafif, taşınabilir bir ilişkisel veri tabanı motoru olan **SQLite**'ı kullanmaktadır. Bu sayede harici bir sunucu kurulumuna ihtiyaç duymadan "tak-çalıştır" (portable) mimaride kararlı bir şekilde çalışır.

---

## 🏗️ Mimari Yapı: MVVM (Model-View-ViewModel)

Proje, arayüz (UI) kodları ile iş mantığını (Business Logic) birbirinden tamamen ayıran, test edilebilir ve sürdürülebilir **MVVM** mimari desenine sadık kalınarak inşa edilmiştir:

* **Model (Veri Katmanı):** Veri tabanındaki tabloları ve iş kurallarını temsil eder. `AppDbContext` üzerinden veri tabanı şemasıyla doğrudan eşleşir.
* **View (Görsel Katman):** Kullanıcı etkileşimini sağlayan `WPF/XAML` arayüzleridir. İçerisinde kesinlikle iş mantığı barındırmaz, tamamen veri bağlama (Data Binding) ile beslenir.
* **ViewModel (Köprü Katmanı):** View ile Model arasında çift yönlü bir iletişim bağı kurar. Görsel katmandan gelen tetiklemeleri alarak iş servislerine aktarır ve arayüzü dinamik olarak günceller.

---

## 📋 Gelişmiş Kurumsal Modüller ve Özellikler

Sistem, gerçek dünya operasyonlarını simüle eden birbiriyle entegre 9 temel modülden oluşmaktadır:

1.  **Gösterge Paneli (Dashboard):** Depodaki toplam ürün sayısını, kategori dağılımlarını, toplam finansal stok değerini ve kritik seviyenin altına düşen ürün adetlerini anlık grafik ve metriklerle görselleştiren merkezi kontrol ekranı.
2.  **Ürün ve Stok Yönetimi:** Ürünlerin SKU (Stock Keeping Unit) barkod takibi, kategori atamaları, renk/beden gibi varyant nitelikleri ve depo içi fiziksel konum eşleşmeleriyle birlikte tam kapsamlı CRUD (Ekleme, Listeleme, Güncelleme, Silme) operasyonları.
3.  **Tedarikçi Zinciri Yönetimi:** Aktif tedarikçilerin firma bilgileri, kurumsal iletişim sorumluları ve tedarikçiye özel ürün geçmişlerinin ilişkilendirilerek takip edildiği modül.
4.  **Satın Alma Siparişleri (Purchase Orders - PO):** Tedarikçilere verilen siparişlerin dijital olarak oluşturulması, maliyet takibi ve siparişlerin "Beklemede/Teslim Alındı" durum süreçlerinin yönetimi.
5.  **Güvenli Stok Giriş/Çıkış Sistemi:** Depoya giren veya depodan çıkan her bir ürün miktarı için birim fiyat, zaman damgası (Timestamp) ve hareket gerekçesini (Giriş, Satış, Zayiat vb.) zorunlu kılan güvenli stok hareket yapısı.
6.  **Kapsamlı İşlem Geçmişi (Transaction Ledger):** Veri tabanında geriye dönük değiştirilemez (immutable) bir şekilde saklanan, denetim standartlarına uygun tüm stok hareketlerinin kronolojik seyir defteri.
7.  **İade ve Hurda (Scrap) Kontrolü:** Kusurlu veya iade gelen ürünlerin kalite kontrol protokolünden geçirilerek yeniden aktif stoğa mı dahil edileceğine yoksa zayiat yazılarak hurda alanına mı aktarılacağına karar veren dinamik mekanizma.
8.  **Fiziksel Konumlandırma (Location Management):** Depo zemin ekonomisini dijitalleştiren; ürünleri Kat (Floor), Bölüm (Section) ve Raf Numarası (Shelf) bazında adresleyerek arama sürelerini sıfıra indiren konum yönetim katmanı.
9.  **Kullanıcı Yetkilendirme ve Güvenlik:** `Admin` (Tam Yetki) ve `Standart Kullanıcı` (Sınırlı Yetki) rollerini barındıran Güvenli Giriş (Login/Logout) sistemi. Yetkisiz personelin kritik butonlara ve silme operasyonlarına erişimi mimari düzeyde engellenir.

---

## 💎 Uygulanan Yazılım Mühendisliği ve Temiz Kod Prensipleri

Bu proje, kod kalitesini en üst düzey kurumsal standartlara taşımak için **SOLID** yazılım ilkelerini ve **Advanced OOP** tekniklerini katı bir şekilde uygular.

### 🧩 Gelişmiş OOP İlkeleri
* **Kapsülleme (Encapsulation):** Modellerdeki ve servislerdeki veri alanları (fields) dış dünyaya doğrudan kapatılmıştır. Örneğin, `Product.cs` içerisindeki `StockQuantity` (Stok Miktarı) değeri dışarıdan doğrudan manipüle edilemez; yalnızca `StockIn()` ve `StockOut()` iş metodları üzerinden güvenli ve kontrollü bir şekilde güncellenebilir.
* **Kalıtım (Inheritance):** Tekrarlayan kod yapılarını engellemek ve merkezi kontrol sağlamak amacıyla ortak özellikler üst sınıflarda toplanmıştır. Her tabloda ortak olan `Id`, `CreatedDate` ve `IsDeleted` alanları `BaseEntity` sınıfından; ViewModel'lerin ortak WPF özellikleri `BaseViewModel`'den; veri tabanı ortak operasyonları ise `BaseRepository` yapısından miras alınır.
* **Soyutlama ve Arayüz Ayrımı (Abstraction & Interface Segregation):** Kullanıcı arayüzü veya ViewModel katmanları asla ham veri tabanı sınıflarıyla doğrudan konuşmaz. İletişim, iş süreçlerine özel olarak tasarlanmış minimal arayüzler (`IRepository<T>`, `IProductService`, `ISupplierService`) üzerinden yürütülür. Bu sayede sistem, alt bileşenlerden tamamen bağımsız (loosely coupled) hale getirilmiştir.

---

## 🧩 Uygulanan Tasarım Desenleri (Design Patterns)

Projenin esnek ve genişletilebilir olmasını sağlamak adına mimaride 3 farklı kategoride **7 adet Tasarım Deseni** aktif olarak kullanılmıştır:



### 1. Oluşturucu (Creational) Desenler
* **Singleton Pattern (`WarehouseManager`):** Uygulama yaşam döngüsü boyunca veri tabanına açılan bağlantı kanalının (`DbContext`) tek bir kanal üzerinden yönetilmesini garanti eder. Bellekte gereksiz nesne oluşumunu engeller ve thread güvenliğini korur.
* **Factory Method Pattern (`ProductFactory`):** Depoya eklenecek farklı ürün türlerinin (örneğin Giyim, Kırtasiye, Tekstil) karmaşık nesne oluşturma ve varsayılan parametre atama süreçlerini tek bir merkezde kapsüller.

### 2. Yapısal (Structural) Desenler
* **Repository Pattern (`BaseRepository`):** Entity Framework Core'a ait ham SQL/LINQ komutlarını business katmanından tamamen soyutlar. Yarın bir gün SQLite yerine SQL Server veya PostgreSQL kullanılmak istendiğinde üst katmanlardaki hiçbir kodu değiştirmeden sadece ilgili Repository sınıfının güncellenmesi yeterli olur.
* **Facade Pattern (`WarehouseManager`):** İçerideki onlarca karmaşık veri tabanı alt sistemini (ProductRepository, SupplierRepository, TransactionRepository vb.) tek bir temiz giriş kapısı (Facade) arkasında birleştirir. ViewModel katmanı yüzlerce bağımlılık yerine sadece bu tek merkezi kapıyla konuşur.
* **Decorator / Converter Pattern (`*Converter.cs`):** WPF mimarisindeki `IValueConverter` yapısını kullanarak veri tabanından gelen saf verileri (örneğin true/false veya sayısal kat bilgileri) orijinal kaynağa dokunmadan, arayüzün ihtiyacına göre dinamik olarak görsel nesnelere (renkli uyarılar, metinsel ifadeler) dönüştürür.

### 3. Davranışsal (Behavioral) Desenler
* **Observer Pattern (`StockAlertSystem`):** Herhangi bir ürünün stok miktarı belirlenen kritik eşiğin altına düştüğü an, sisteme kayıtlı olan aboneler (`LogObserver` arka plana txt/veri tabanı günlüğü yazar, `UIAlertObserver` ise ekrana anlık kırmızı uyarı fırlatır) olay tabanlı (event-driven) olarak otomatik olarak tetiklenir.
* **Command Pattern (`RelayCommand`):** WPF üzerindeki buton tıklama ve kullanıcı aksiyonlarını doğrudan iş metodlarına bağlar. Bunu yapırken "Guard Condition" (Muhafız Koşulu) uygulayarak, örneğin metin kutuları boşsa veya kullanıcının yetkisi yoksa butonun arayüzde otomatik olarak kilitlenmesini (disabled) sağlar.
* **Strategy Pattern (`ReportService`):** Sistemden dışa veri aktarımı yapılırken (örneğin CSV aktarımı, Excel aktarımı veya Metin belgesi aktarımı) uygulanacak raporlama algoritmalarını birbirinden bağımsız stratejiler olarak soyutlar ve çalışma zamanında (runtime) dinamik olarak değiştirilmesine imkan tanır.
* **Template Method Pattern (`BaseRepository.Update`):** Veri tabanı güncelleme algoritmasının genel iskeletini üst sınıfta kurar, somut sınıfların (örneğin `ProductRepository`) sadece kendilerine has olan özel güncelleme adımlarını (override) doldurmasına izin verir.

---

## 🛠️ Teknolojik Altyapı ve Bağımlılıklar

* **Dil ve Çerçeve:** C# / .NET 10.0 / WPF (XAML)
* **ORM ve Veri Tabanı:** Entity Framework Core (EF Core) / SQLite (Veri tabanı ilk çalıştırmada otomatik oluşur, sunucu kurulumu gerektirmez).
* **Arayüz Bileşenleri:** WPF Gelişmiş Kaynak Sözlükleri ve Modern Material Design Bileşenleri.

---

## 🚀 Kurulum ve Çalıştırma Talimatı

1.  Bu repository'yi bilgisayarınıza indirin veya klonlayın.
2.  Visual Studio editörünü açarak ana dizindeki `WarehouseManagementSystem.slnx` yeni nesil çözüm (solution) dosyasını çift tıklayarak yükleyin.
3.  Gerekli NuGet paketlerinin (**Microsoft.EntityFrameworkCore.Sqlite**, **MaterialDesignThemes**) otomatik indirilebilmesi için projeyi derleyin (`Build Solution` veya `Ctrl+Shift+B`).
4.  Uygulamayı yerel olarak başlatmak için `F5` tuşuna basın. Sistem ilk açılışta SQLite veritabanı dosyasını projenin çalışma dizininde otomatik olarak üretecektir.
