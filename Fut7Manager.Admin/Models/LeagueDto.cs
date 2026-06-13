using Fut7Manager.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class LeagueDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public decimal RegistrationFee { get; set; }

        public LeagueStatus Status { get; set; }

        public string? LogoUrl { get; set; }

        public bool UsePenaltyShootoutPoints { get; set; }

        public int TotalQualifiedTeams { get; set; }

        public int? MinPlayers { get; set; }
    }
}
