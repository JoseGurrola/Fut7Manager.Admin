using Fut7Manager.Admin.ViewModels;
using Fut7Manager.Admin.ViewModels.Common;
using Fut7Manager.Admin.Views;
using Fut7Manager.Admin.Views.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Services {
    public static class MessageService {
        public static void Show(
            string message,
            string title = "Mensaje") {

            var dialog = new CustomMessageBox {
                DataContext = new CustomMessageBoxViewModel(
                    title,
                    message)
            };

            dialog.ShowDialog();
        }

        public static bool Confirm(
            string message,
            string title = "Confirmar") {

            var dialog = new ConfirmDialog {
                DataContext = new ConfirmDialogViewModel(message)
            };

            return dialog.ShowDialog() == true;
        }
    }
}
