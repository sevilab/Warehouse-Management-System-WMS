namespace WarehouseManagementSystem.Interfaces
{
    // Generic Repository Pattern — tüm repository'lerin uyması gereken sözleşme
    // OOP: Abstraction

    // Interfaces/IRepository.cs
    public interface IRepository<T> where T : class
    {
        void Add(T entity); // "Ekle" nasıl eklendiği gizli
        void Update(T entity);// "Güncelle" nasıl güncellendiği gizli
        void Delete(int id);// "Sil" nasıl silindiği gizli
        T? GetById(int id); // "Bul" nasıl bulunduğu gizli
        IEnumerable<T> GetAll(); 
        IEnumerable<T> GetAllActive();
    }
}