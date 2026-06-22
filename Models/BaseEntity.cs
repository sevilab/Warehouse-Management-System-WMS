namespace WarehouseManagementSystem.Models
{
    // Tüm entity sınıflarının türeyeceği abstract temel sınıf
    // OOP: Abstraction + Inheritance


    //  TEMEL SINIF
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // KAPSÜLLENMİŞ — Sadece bu sınıf ve miras alanlar set edebilir
        public bool IsDeleted { get; protected set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsDeleted = false;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
        }

        protected void SetUpdatedAt()
        {
            UpdatedAt = DateTime.Now;
        }

        // Polymorphism Models/BaseEntity.cs — genel versiyon 
        public override string ToString()
        {
            return $"[{GetType().Name}] Id: {Id} | Created: {CreatedAt:dd.MM.yyyy}";
        }
    }
}