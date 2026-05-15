using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class FinalizeLeagueSetupDto {
        public bool InterGroupMatches { get; set; }
        public List<TeamGroupAssignmentDto> Teams { get; set; } = new();
    }
}
