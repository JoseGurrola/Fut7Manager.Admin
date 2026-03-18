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
    /// Interaction logic for LeaguesView.xaml
    /// </summary>
    public partial class LeaguesView : UserControl {
        public LeaguesView() {
            InitializeComponent();
            //DataContext = new LeaguesViewModel();
        }
    }
}
