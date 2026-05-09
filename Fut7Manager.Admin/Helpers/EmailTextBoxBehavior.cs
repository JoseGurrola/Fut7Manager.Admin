using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Fut7Manager.Admin.Helpers {
    public static class EmailTextBoxBehavior {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(EmailTextBoxBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value)
            => element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(IsEnabledProperty);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBox textBox) {
                if ((bool)e.NewValue)
                    textBox.LostFocus += Validate;
                else
                    textBox.LostFocus -= Validate;
            }
        }

        private static void Validate(object sender, RoutedEventArgs e) {
            var tb = sender as TextBox;
            if (tb == null) return;

            var email = tb.Text;

            if (string.IsNullOrWhiteSpace(email)) {
                ClearError(tb);
                return;
            }

            var isValid = Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!isValid)
                SetError(tb, "Email inválido");
            else
                ClearError(tb);
        }

        private static void SetError(TextBox tb, string msg) {
            System.Windows.Controls.Validation.MarkInvalid(
                System.Windows.Data.BindingOperations.GetBindingExpression(tb, TextBox.TextProperty),
                new System.Windows.Controls.ValidationError(
                    new DataErrorValidationRule(), tb, msg, null));
        }

        private static void ClearError(TextBox tb) {
            var binding = System.Windows.Data.BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
            if (binding != null)
                System.Windows.Controls.Validation.ClearInvalid(binding);
        }
    }
}
