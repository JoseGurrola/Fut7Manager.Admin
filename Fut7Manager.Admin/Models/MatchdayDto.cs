using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class MatchdayDto {
        public int Id { get; set; }
        public int Number { get; set; }
        public List<Fut7MatchDto> Matches { get; set; } = new();

        public List<string> RestingTeamNames { get; set; } = new();

        // 🔥 DISPLAY 
        public string RestingTeamsDisplay =>
            RestingTeamNames != null && RestingTeamNames.Any()
                ? $"😴 Descansa: {string.Join(", ", RestingTeamNames)}"
                : string.Empty;
    }
}
