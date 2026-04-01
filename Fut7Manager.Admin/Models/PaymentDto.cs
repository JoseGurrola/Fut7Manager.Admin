using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class PaymentDto {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
