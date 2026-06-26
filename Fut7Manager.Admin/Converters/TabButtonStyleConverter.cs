using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Fut7Manager.Admin.Converters
{
    public class TabButtonStyleConverter : IMultiValueConverter {
        // values[0] = SelectedTab (enum)
        // values[1] = CommandParameter (string)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return new Thickness(0, 0, 0, 2);

            string selected = values[0].ToString() ?? string.Empty;
            string param = values[1].ToString() ?? string.Empty;

            if (selected == param) {
                // Activo: borde inferior más grueso
                return new Thickness(0, 0, 0, 3);
            }

            // Inactivo: borde normal
            return new Thickness(0, 0, 0, 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TabButtonForegroundConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return Brushes.Gray;

            string selected = values[0].ToString() ?? string.Empty;
            string param = values[1].ToString() ?? string.Empty;

            return selected == param ? Brushes.Black : Brushes.Gray;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TabButtonBorderConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return new SolidColorBrush(Colors.Gray);

            string selected = values[0].ToString() ?? string.Empty;
            string param = values[1].ToString() ?? string.Empty;

            // Si coincide, azul; si no, gris
            return selected == param ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : new SolidColorBrush(Colors.Gray);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
