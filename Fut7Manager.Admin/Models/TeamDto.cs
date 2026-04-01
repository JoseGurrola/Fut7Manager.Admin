using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class TeamDto {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? LogoUrl { get; set; }

        public int PositionTable { get; set; }

        public int Points { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }

        public string PaymentStatus
        {
            get {
                if (Remaining <= 0)
                    return "Paid";

                if (Paid > 0)
                    return "Partial";

                return "Due";
            }
        }

        public string TeamManagerName { get; set; } = default!;

        public string TeamManagerPhone { get; set; } = default!;

        public int? GroupId { get; set; }
        public int LeagueId { get; set; }
    }
}
