using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class PaymentSummaryDto {
        public decimal TotalDue { get; set; }      // Total que deberían pagar todos los equipos
        public decimal TotalPaid { get; set; }     // Total pagado por todos los equipos
        public decimal PercentagePaid { get; set; } // Porcentaje global pagado
    }
}
