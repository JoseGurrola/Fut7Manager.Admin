using Fut7Manager.Admin.Models;
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
    /// Interaction logic for MatchesView.xaml
    /// </summary>
    public partial class MatchesView : UserControl{
        public MatchesView() {
            InitializeComponent();

            //DataContext = new MatchesViewModel();
        }

        private async void Match_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            // solo doble click
            if (e.ClickCount != 2)
                return;

            if (DataContext is not MatchesViewModel vm)
                return;

            if (sender is not Border border)
                return;

            if (border.Tag is not Fut7MatchDto match)
                return;

            var window = new EditMatchWindow();

            var editVm = new EditMatchViewModel(
                match,
                vm.Fut7MatchService,
                vm.League);

            window.DataContext = editVm;

            editVm.CloseAction = async (result) => {

                window.DialogResult = result;
                window.Close();

                if (result) {
                    await vm.UpdateMatch(match);
                }
            };

            window.ShowDialog();
        }
    }
}
