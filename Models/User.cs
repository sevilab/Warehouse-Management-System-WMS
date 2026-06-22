namespace WarehouseManagementSystem.Models
{
    public enum UserRole
    {
        Admin,
        Kullanici
    }

    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        public User() { }

        public User(string username, string password,
                    string fullName, UserRole role)
        {
            Username = username;
            PasswordHash = HashPasswordStatic(password);
            FullName = fullName;
            Role = role;
            IsActive = true;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == HashPasswordStatic(password);
        }

        public static string HashPasswordStatic(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public override string ToString()
        {
            return $"{FullName} ({Username}) — {Role}";
        }
    }
}