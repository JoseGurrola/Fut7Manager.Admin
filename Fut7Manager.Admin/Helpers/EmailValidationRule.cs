using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Fut7Manager.Admin.Helpers {
    public class EmailValidationRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            string email = value as string;

            if (string.IsNullOrWhiteSpace(email))
                return ValidationResult.ValidResult;

            bool ok = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return ok
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Email inválido");
        }
    }
}
