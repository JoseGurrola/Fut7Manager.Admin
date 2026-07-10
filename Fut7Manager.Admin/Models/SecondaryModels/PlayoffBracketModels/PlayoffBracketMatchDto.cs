using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Fut7Manager.Admin.Models.SecondaryModels.PlayoffBracketModels
{
    public class PlayoffBracketMatchDto {
        public int MatchNumber { get; set; }

        public PlayoffTeamDto? HomeTeam { get; set; }

        public PlayoffTeamDto? AwayTeam { get; set; }

        public int? HomeGoals { get; set; }

        public int? AwayGoals { get; set; }

        public double MarginBottom { get; set; }

        public Thickness MatchMargin =>
    new Thickness(0, 0, 0, MarginBottom);
    }
}
