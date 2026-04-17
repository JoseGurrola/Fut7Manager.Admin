using System;
using System.Collections.Generic;
using System.Text;
using static Fut7Manager.Admin.ViewModels.CentralPanelViewModel;

namespace Fut7Manager.Admin.Models {
    public class LeagueDashboardDto {
        public MatchdayDto CurrentMatchday { get; set; } = default!;

        public List<GroupStandingDto> GroupedStandings { get; set; } = default!;

        public List<StandingDto> Standings { get; set; } = default!;
    }
}
