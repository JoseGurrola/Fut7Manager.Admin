using Fut7Manager.Admin.Helpers;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class ConfirmDialogViewModel : BaseViewModel {
        public string Message { get; }

        public ICommand ConfirmCommand { get; }

        public ConfirmDialogViewModel(string message) {
            Message = message;
            ConfirmCommand = new RelayCommand(OnConfirm);
        }

        private void OnConfirm() {
            if (Application.Current.Windows.Count == 0)
                return;

            foreach (Window window in Application.Current.Windows) {
                if (window is Views.ConfirmDialog) {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }
    }
}
