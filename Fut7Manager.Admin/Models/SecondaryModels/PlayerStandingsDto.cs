using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels {
    public class PlayerStandingsDto {
        public List<PlayerStatStandingDto> TopScorers { get; set; } = new();
        public List<PlayerStatStandingDto> YellowCards { get; set; } = new();
        public List<PlayerStatStandingDto> RedCards { get; set; } = new();
    }
}
