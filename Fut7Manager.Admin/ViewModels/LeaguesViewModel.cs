using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.ViewModels {
    public class LeaguesViewModel : BaseViewModel {
        private readonly LeagueService _leagueService;

        public ObservableCollection<LeagueDto> Leagues { get; set; }

        public LeaguesViewModel() {
            _leagueService = new LeagueService();
            Leagues = new ObservableCollection<LeagueDto>();

           _ = LoadLeagues();
        }

        private async Task LoadLeagues() {
            var _leagues = await _leagueService.GetLeaguesAsync();

            Leagues.Clear();

            foreach (var league in _leagues) {
                Leagues.Add(league);
            }
        }
    }
}
