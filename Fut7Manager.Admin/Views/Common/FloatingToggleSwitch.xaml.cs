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

namespace Fut7Manager.Admin.Views.Common {
    /// <summary>
    /// Interaction logic for FloatingToggleSwitch.xaml
    /// </summary>
    public partial class FloatingToggleSwitch : UserControl {
        public FloatingToggleSwitch() {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsCheckedProperty =
           DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(FloatingToggleSwitch),
               new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly DependencyProperty StateLabel2TextProperty =
    DependencyProperty.Register(nameof(StateLabel2Text), typeof(string), typeof(FloatingToggleSwitch),
        new FrameworkPropertyMetadata("SI", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string StateLabel2Text
        {
            get => (string)GetValue(StateLabel2TextProperty);
            set => SetValue(StateLabel2TextProperty, value);
        }
    }
}
