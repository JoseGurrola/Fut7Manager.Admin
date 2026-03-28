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

namespace Fut7Manager.Admin.Views
{
    /// <summary>
    /// Interaction logic for CreateLeagueWindow.xaml
    /// </summary>
    public partial class CreateLeagueWindow : Window {
        public CreateLeagueWindow() {
            InitializeComponent();

            Loaded += CreateLeagueWindow_Loaded;
        }

        private void CreateLeagueWindow_Loaded(object sender, RoutedEventArgs e) {
            if (DataContext is CreateOrEditLeagueViewModel vm) {
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
