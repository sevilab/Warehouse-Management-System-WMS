using System.Collections.ObjectModel;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Repositories;

namespace WarehouseManagementSystem.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        private ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                if (value != null) FillForm(value);
            }
        }

        // Form alanları
        private string _formUsername = string.Empty;
        public string FormUsername
        {
            get => _formUsername;
            set => SetProperty(ref _formUsername, value);
        }

        private string _formFullName = string.Empty;
        public string FormFullName
        {
            get => _formFullName;
            set => SetProperty(ref _formFullName, value);
        }

        private string _formPassword = string.Empty;
        public string FormPassword
        {
            get => _formPassword;
            set => SetProperty(ref _formPassword, value);
        }

        private UserRole _formRole;
        public UserRole FormRole
        {
            get => _formRole;
            set => SetProperty(ref _formRole, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isStatusError;
        public bool IsStatusError
        {
            get => _isStatusError;
            set => SetProperty(ref _isStatusError, value);
        }

        public List<UserRole> Roles { get; } = Enum.GetValues<UserRole>().ToList();

        // Komutlar
        public RelayCommand AddUserCommand { get; private set; }
        public RelayCommand DeleteUserCommand { get; private set; }
        public RelayCommand ChangePasswordCommand { get; private set; }
        public RelayCommand ClearFormCommand { get; private set; }

        public UserViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;

            AddUserCommand = new RelayCommand(_ => AddUser());
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(),
                _ => SelectedUser != null);
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword(),
                _ => SelectedUser != null && !string.IsNullOrWhiteSpace(FormPassword));
            ClearFormCommand = new RelayCommand(_ => ClearForm());

            Refresh();
        }

        public void Refresh()
        {
            var list = _userRepository.GetAllActive().ToList();
            Users = new ObservableCollection<User>(list);
        }

        private void FillForm(User user)
        {
            FormUsername = user.Username;
            FormFullName = user.FullName;
            FormRole = user.Role;
            FormPassword = string.Empty;
            StatusMessage = string.Empty;
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(FormUsername))
            { SetError("Kullanıcı adı boş olamaz!"); return; }
            if (string.IsNullOrWhiteSpace(FormFullName))
            { SetError("Ad Soyad boş olamaz!"); return; }
            if (string.IsNullOrWhiteSpace(FormPassword))
            { SetError("Şifre boş olamaz!"); return; }
            if (FormPassword.Length < 6)
            { SetError("Şifre en az 6 karakter olmalıdır!"); return; }
            if (_userRepository.UsernameExists(FormUsername))
            { SetError($"'{FormUsername}' kullanıcı adı zaten mevcut!"); return; }

            try
            {
                var user = new User(FormUsername, FormPassword,
                    FormFullName, FormRole);
                _userRepository.Add(user);
                SetSuccess($"✅ '{FormFullName}' kullanıcısı eklendi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;
            if (SelectedUser.Username == "admin")
            { SetError("Ana admin hesabı silinemez!"); return; }

            try
            {
                string name = SelectedUser.FullName;
                _userRepository.Delete(SelectedUser.Id);
                SetSuccess($"✅ '{name}' silindi.");
                ClearForm();
                Refresh();
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void ChangePassword()
        {
            if (SelectedUser == null) return;
            if (FormPassword.Length < 6)
            { SetError("Şifre en az 6 karakter olmalıdır!"); return; }

            try
            {
                SelectedUser.PasswordHash = User.HashPasswordStatic(FormPassword);
                SelectedUser.UpdatedAt = DateTime.Now;
                _userRepository.Update(SelectedUser);
                SetSuccess($"✅ Şifre güncellendi.");
                FormPassword = string.Empty;
            }
            catch (Exception ex)
            {
                SetError($"Hata: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            FormUsername = string.Empty;
            FormFullName = string.Empty;
            FormPassword = string.Empty;
            FormRole = UserRole.Kullanici;
            SelectedUser = null;
            StatusMessage = string.Empty;
        }

        private void SetError(string message)
        {
            StatusMessage = "❌ " + message;
            IsStatusError = true;
        }

        private void SetSuccess(string message)
        {
            StatusMessage = message;
            IsStatusError = false;
        }
    }
}