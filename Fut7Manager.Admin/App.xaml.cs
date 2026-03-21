using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels;
using Fut7Manager.Admin.Views;
using System.Windows;

namespace Fut7Manager.Admin {
    public partial class App : Application {
        public static AppState State { get; } = new AppState();

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var loginVm = new LoginViewModel();
            var loginWindow = new LoginWindow {
                DataContext = loginVm
            };

            loginVm.LoginSucceeded += async () =>
            {
                var mainVm = new MainViewModel(State);
                var mainWindow = new MainWindow { DataContext = mainVm };
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                // Abrir automáticamente selección de liga tras login
                await mainVm.OpenLeagueSelectionAfterLoginAsync();

                loginWindow.Close();
            };

            loginWindow.Show();
        }
    }
}