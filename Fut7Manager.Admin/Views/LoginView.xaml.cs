using System.Windows.Controls;

namespace Fut7Manager.Admin.Views {
    public partial class LoginView : UserControl {
        public LoginView() {
            InitializeComponent();

            PasswordBox.PasswordChanged += (s, e) => {
                if (DataContext is null) return;

                var prop = DataContext.GetType().GetProperty("Password");
                if (prop != null && prop.CanWrite) {
                    prop.SetValue(DataContext, PasswordBox.Password);
                }
            };
        }
    }
}