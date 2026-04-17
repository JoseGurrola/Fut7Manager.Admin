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

namespace Fut7Manager.Admin.Views.Common
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TeamLogo : UserControl {
        public TeamLogo() {
            InitializeComponent();
        }

        public ImageSource? Logo
        {
            get => (ImageSource)GetValue(LogoProperty);
            set => SetValue(LogoProperty, value);
        }

        public static readonly DependencyProperty LogoProperty =
    DependencyProperty.Register(nameof(Logo), typeof(ImageSource), typeof(TeamLogo),
        new PropertyMetadata(null));

       
    }
}
