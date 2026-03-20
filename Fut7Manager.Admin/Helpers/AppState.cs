using Fut7Manager.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Helpers {
    public class AppState {
        public LeagueDto? SelectedLeague { get; private set; }

        public event Action? LeagueChanged;

        public void SetLeague(LeagueDto league) {
            SelectedLeague = league;
            LeagueChanged?.Invoke();
        }

        public void ClearLeague() {
            SelectedLeague = null;
            LeagueChanged?.Invoke();
        }
    }
}