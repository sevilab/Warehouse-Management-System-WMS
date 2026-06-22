using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private User? _currentUser;

        public User? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;
        public bool IsAdmin => _currentUser?.Role == UserRole.Admin;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
            EnsureDefaultUsers();
        }

        private void EnsureDefaultUsers()
        {
            if (!_userRepository.GetAll().Any())
            {
                var admin = new User("admin", "admin123",
                    "Sistem Yöneticisi", UserRole.Admin);
                var user = new User("kullanici", "kullanici123",
                    "Depo Görevlisi", UserRole.Kullanici);

                _userRepository.Add(admin);
                _userRepository.Add(user);
            }
        }

        public bool Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null) return false;
            if (!user.VerifyPassword(password)) return false;

            _currentUser = user;
            return true;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool HasPermission(string action)
        {
            if (_currentUser == null) return false;
            if (_currentUser.Role == UserRole.Admin) return true;

            // Kullanıcı rolü için izinler
            return action switch
            {
                "StockIn" => true,
                "StockOut" => true,
                "ViewProducts" => true,
                "ViewSuppliers" => true,
                "ViewTransactions" => true,
                "ViewDashboard" => true,
                _ => false
            };
        }
    }
}