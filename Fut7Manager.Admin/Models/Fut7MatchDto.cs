using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class Fut7MatchDto {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public string HomeTeamName { get; set; } = default!;

        public string AwayTeamName { get; set; } = default!;

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public DateTime? MatchDate { get; set; }

        public string? Location { get; set; }

        public int LeagueId { get; set; }

        public int? MatchdayId { get; set; }

        public int MatchdayNumber { get; set; }

        public string? HomeTeamLogo { get; set; }
        public string? AwayTeamLogo { get; set; }

        public string ScoreDisplay =>
    HomeGoals.HasValue && AwayGoals.HasValue
        ? $"{HomeGoals} - {AwayGoals}"
        : "vs";

        public string DateDisplay =>
            MatchDate.HasValue
                ? MatchDate.Value.ToString("dd/MM/yyyy HH:mm")
                : "Sin programar";

        public string LocationDisplay =>
            string.IsNullOrEmpty(Location)
                ? ""
                : Location;
    }
}
