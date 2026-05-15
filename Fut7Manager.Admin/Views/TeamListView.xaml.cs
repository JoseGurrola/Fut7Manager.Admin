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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fut7Manager.Admin.Views {
    /// <summary>
    /// Interaction logic for TeamsView.xaml
    /// </summary>
    public partial class TeamListView : UserControl {
        public TeamListView() {
            InitializeComponent();
        }

        private void TeamsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (DataContext is TeamListViewModel vm)
                vm.OpenTeamCommand.Execute(null);
        }
    }

}
