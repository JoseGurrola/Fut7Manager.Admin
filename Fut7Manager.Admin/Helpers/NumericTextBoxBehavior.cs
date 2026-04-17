using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fut7Manager.Admin.Helpers {
    public static class NumericTextBoxBehavior {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(NumericTextBoxBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.RegisterAttached(
                "DecimalPlaces",
                typeof(int),
                typeof(NumericTextBoxBehavior),
                new PropertyMetadata(2));

        public static readonly DependencyProperty OnlyDigitsProperty = DependencyProperty.RegisterAttached(
                "OnlyDigits",
                typeof(bool),
                typeof(NumericTextBoxBehavior),
                new PropertyMetadata(false));

        private static readonly Regex _regex = new Regex(@"^[0-9]*\.?[0-9]*$");

        public static bool GetIsEnabled(DependencyObject obj)
            => (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value)
            => obj.SetValue(IsEnabledProperty, value);

        public static int GetDecimalPlaces(DependencyObject obj)
            => (int)obj.GetValue(DecimalPlacesProperty);

        public static void SetDecimalPlaces(DependencyObject obj, int value)
            => obj.SetValue(DecimalPlacesProperty, value);

        private static readonly Regex _digitsRegex = new Regex(@"^[0-9]*$");

        public static bool GetOnlyDigits(DependencyObject obj)
            => (bool)obj.GetValue(OnlyDigitsProperty);

        public static void SetOnlyDigits(DependencyObject obj, bool value)
            => obj.SetValue(OnlyDigitsProperty, value);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBox tb) {
                if ((bool)e.NewValue) {
                    tb.PreviewTextInput += OnPreviewTextInput;
                    tb.PreviewKeyDown += OnPreviewKeyDown;
                    tb.LostFocus += OnLostFocus;
                } else {
                    tb.PreviewTextInput -= OnPreviewTextInput;
                    tb.PreviewKeyDown -= OnPreviewKeyDown;
                    tb.LostFocus -= OnLostFocus;
                }
            }
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
            var tb = sender as TextBox;
            if (tb is null) return;
            string newText = tb.Text.Insert(tb.SelectionStart, e.Text);

            bool onlyDigits = GetOnlyDigits(tb);

            if (onlyDigits) {
                if (!_digitsRegex.IsMatch(newText)) {
                    e.Handled = true;
                }
                return;
            }

            if (!_regex.IsMatch(newText)) {
                e.Handled = true;
                return;
            }

            int decimalPlaces = GetDecimalPlaces(tb);
            if (newText.Contains(".")) {
                var parts = newText.Split('.');
                if (parts.Length > 1 && parts[1].Length > decimalPlaces)
                    e.Handled = true;
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e) {
            if (sender is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text)) {
                if (GetOnlyDigits(tb))
                    return;

                if (decimal.TryParse(tb.Text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var value)) {
                    int decimals = GetDecimalPlaces(tb);
                    tb.Text = value.ToString("N" + decimals, CultureInfo.InvariantCulture);
                }
            }
        }
    }
}