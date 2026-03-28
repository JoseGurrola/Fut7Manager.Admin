using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fut7Manager.Admin {
    public class BaseViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        protected bool SetProperty<T>(ref T field, T value,
            [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}