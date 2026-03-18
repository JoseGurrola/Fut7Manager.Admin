using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class LoginViewModel : BaseViewModel {
        private readonly AuthService _authService;

        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public ICommand LoginCommand { get; }

        public Action? OnLoginSuccess { get; set; }

        public LoginViewModel() {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async () => await Login());
        }

        private async Task Login() {
            var token = await _authService.LoginAsync(Username, Password);

            if (token != null) {
                TokenStorage.Token = token;

                OnLoginSuccess?.Invoke();
            } else {
                System.Diagnostics.Debug.WriteLine("Login fallido");
            }
        }
    }
}
