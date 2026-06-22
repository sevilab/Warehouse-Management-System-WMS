using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public RelayCommand LoginCommand { get; private set; }

        public event Action? LoginSuccessful;

        public string Password { get; set; } = string.Empty;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(_ => DoLogin());
        }

        private void DoLogin()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                SetError("Kullanıcı adı boş olamaz!");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                SetError("Şifre boş olamaz!");
                return;
            }

            IsLoading = true;
            HasError = false;

            bool success = _authService.Login(Username, Password);

            IsLoading = false;

            if (success)
                LoginSuccessful?.Invoke();
            else
                SetError("Kullanıcı adı veya şifre hatalı!");
        }

        private void SetError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }
    }
}