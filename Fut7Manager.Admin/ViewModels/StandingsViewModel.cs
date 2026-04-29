using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.ViewModels
{
    public class StandingsViewModel : BaseViewModel {
        private LeagueDto _league;
        private readonly LeagueService _leagueService;

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }
        private bool _isLoading;

        public ObservableCollection<GroupStandingDto> GroupedStandings { get; } = new();
        public ObservableCollection<StandingDto> GeneralStandings { get; } = new();

        public StandingsViewModel(AppState appState, LeagueService leagueService, LeagueDto league, TeamService teamService, GroupService groupService, Fut7MatchService fut7MatchService) {
            _league = league;
            _leagueService = new LeagueService();
        }

        public async Task InitializeAsync() {
            await LoadStandings();
        }

        private async Task LoadStandings() {
            IsLoading = true;

            var result = await _leagueService.GetStandingsAsync(_league.Id);

            GroupedStandings.Clear();
            GeneralStandings.Clear();

            if (result != null) {
                foreach (var group in result.GroupedStandings)
                    GroupedStandings.Add(group);

                foreach (var team in result.Standings)
                    GeneralStandings.Add(team);
            }

            IsLoading = false;
        }
    }
}
