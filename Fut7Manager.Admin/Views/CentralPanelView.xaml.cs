using Fut7Manager.Admin.ViewModels;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fut7Manager.Admin.Views {
    /// <summary>
    /// Interaction logic for TeamsView.xaml
    /// </summary>
    public partial class CentralPanelView : UserControl {
        public CentralPanelView() {
            InitializeComponent();
        }

        private async void MatchesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (DataContext is not CentralPanelViewModel vm) return;
            if (vm.SelectedMatch == null) return;

            var window = new EditMatchWindow();

            var editVm = new EditMatchViewModel(vm.SelectedMatch, vm.Fut7MatchService);

            window.DataContext = editVm;

            editVm.CloseAction = async (result) =>
            {
                window.DialogResult = result;
                window.Close();

                if (result) {
                    await vm.RefreshDashboard(); // 👈 importante
                }
            };

            window.ShowDialog();
        }
    }

}
