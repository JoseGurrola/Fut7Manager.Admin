using Fut7Manager.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.Common {
    public class CustomMessageBoxViewModel : BaseViewModel {
        public string Title { get; }
        public string Message { get; }

        public ICommand AcceptCommand { get; }

        public CustomMessageBoxViewModel(
            string title,
            string message) {

            Title = title;
            Message = message;

            AcceptCommand = new RelayCommand(OnAccept);
        }
        
        private void OnAccept() {

            foreach (Window window in Application.Current.Windows) {

                if (window is Views.Common.CustomMessageBox) {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }
    }
}
