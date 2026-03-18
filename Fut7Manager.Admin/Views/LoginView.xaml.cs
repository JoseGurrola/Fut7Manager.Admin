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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {
        public LoginView() {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is LoginViewModel vm) {
                    PasswordBox.PasswordChanged += (sender, args) =>
                    {
                        vm.Password = PasswordBox.Password;
                    };
                }
            };
        }
    }
}
