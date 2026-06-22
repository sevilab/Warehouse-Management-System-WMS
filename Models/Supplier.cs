namespace WarehouseManagementSystem.Models
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        public Supplier() { }
        public Supplier(string name, string contactPerson,
                        string email, string phone, string address)
        {
            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
                throw new ArgumentException("Geçersiz email adresi.");
            if (!string.IsNullOrEmpty(phone) && phone.Length < 10)
                throw new ArgumentException("Geçersiz telefon numarası.");

            Name = name;
            ContactPerson = contactPerson;
            Email = email;
            Phone = phone;
            Address = address;
            IsActive = true;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateInfo(string name, string contactPerson,
                               string email, string phone, string address)
        {
            Name = name;
            ContactPerson = contactPerson;
            Email = email;
            Phone = phone;
            Address = address;
            UpdatedAt = DateTime.Now;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.Now;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }

        public override string ToString() // Models/Supplier.cs — tedarikçiye özel versiyon
        {
            return $"{Name} | İletişim: {ContactPerson} | Tel: {Phone}";
        }
    }
}