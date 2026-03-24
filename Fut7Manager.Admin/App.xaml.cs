using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels;
using Fut7Manager.Admin.Views;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Fut7Manager.Admin {
    public partial class App : Application {
        public static AppState State { get; } = new AppState();

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var mainVm = new MainViewModel(App.State);
            var main = new MainWindow {
                DataContext = mainVm
            };

            Application.Current.MainWindow = main;
            main.Show();
        }

        public App() {
            RenderOptions.ProcessRenderMode = RenderMode.Default;
        }
    }
}