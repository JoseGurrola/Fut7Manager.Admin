using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Fut7Manager.Admin {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static AppState State { get; } = new AppState();

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var loginVm = new LoginViewModel();
            var loginWindow = new LoginWindow {
                DataContext = loginVm
            };

            loginVm.LoginSucceeded += () =>
            {
                var mainVm = new MainViewModel(State);
                var main = new MainWindow {
                    DataContext = mainVm
                };

                Application.Current.MainWindow = main;
                main.Show();

                _ = mainVm.InitializeAsync();

                loginWindow.Close();
            };

            loginWindow.Show();
        }
    }
}