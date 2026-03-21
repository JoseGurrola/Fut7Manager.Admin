using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class LoginViewModel : BaseViewModel {
        private readonly AuthService _authService;

        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public event Action? LoginSucceeded;

        public LoginViewModel() {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async () => await Login());
        }

        private async Task Login() {
            ErrorMessage = "";
            IsLoading = true;

            var result = await _authService.LoginAsync(Username, Password);

            IsLoading = false;

            if (result.Success && result.Token != null) {
                TokenStorage.Token = result.Token; // ✅ Funciona ahora
                LoginSucceeded?.Invoke();
            } else {
                ErrorMessage = result.Error ?? "Usuario o contraseña incorrectos.";
            }
        }
    }
}