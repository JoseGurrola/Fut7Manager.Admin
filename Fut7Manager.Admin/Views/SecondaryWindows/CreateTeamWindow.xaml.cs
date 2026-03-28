using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels;
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
using System.Windows.Shapes;

namespace Fut7Manager.Admin.Views.SecondaryWindows {
    /// <summary>
    /// Interaction logic for CreateLeagueWindow.xaml
    /// </summary>
    public partial class CreateTeamWindow : Window {
        public CreateTeamWindow() {
            InitializeComponent();

            Loaded += CreateTeamWindow_Loaded;
        }

        private void CreateTeamWindow_Loaded(object sender, RoutedEventArgs e) {
            if (DataContext is CreateOrEditTeamViewModel vm) {
                vm.CloseAction = result => {
                    DialogResult = result;
                    Close();
                };
            }

            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
