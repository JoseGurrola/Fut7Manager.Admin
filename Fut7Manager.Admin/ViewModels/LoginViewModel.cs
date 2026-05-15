using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class LoginViewModel : BaseViewModel {
        private readonly AuthService _authService = new AuthService();
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public bool IsLoading { get; set; }

        public ICommand LoginCommand { get; }

        private readonly Action LoginSucceededCallback;

        public LoginViewModel(Action loginSucceededCallback) {
            LoginSucceededCallback = loginSucceededCallback;
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        public async Task AutoLoginIfDebugAsync() {
#if DEBUG
            Username = "admin";
            Password = "1234";

            await LoginAsync();
#endif
        }

        private async Task LoginAsync() {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            ErrorMessage = "";

            var result = await _authService.LoginAsync(Username, Password);
            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));

            if (result.Success && result.Token != null) {
                TokenStorage.Token = result.Token;
                LoginSucceededCallback.Invoke();
                SessionManager.IsSessionExpiredHandled = false;
            } else {
                ErrorMessage = result.Error ?? "Usuario o contraseña incorrectos.";
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }
}