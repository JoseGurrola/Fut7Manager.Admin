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
                PasswordBox.PasswordChanged += (sender, args) =>
                {
                    var dc = DataContext;
                    if (dc == null)
                        return;

                    var prop = dc.GetType().GetProperty("Password");
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(dc, PasswordBox.Password);
                    }
                };
            };
        }
    }
}
