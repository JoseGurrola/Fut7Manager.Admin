using Fut7Manager.Admin.Models.SecondaryModels;
using System;
using System.Collections.Generic;
using System.Text;
using static Fut7Manager.Admin.ViewModels.CentralPanelViewModel;

namespace Fut7Manager.Admin.Models
{
    public class StandingsResponseDto {
        public MatchdayDto CurrentMatchday { get; set; } = default!;
        public List<GroupStandingDto> GroupedStandings { get; set; } = new();
        public List<StandingDto> Standings { get; set; } = new();

        public PlayerStandingsDto PlayerStandings { get; set; } = new();
    }
}
