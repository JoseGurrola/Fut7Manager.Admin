using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;

namespace Fut7Manager.Admin.ViewModels {
    public class TeamsViewModel : BaseViewModel {
        private readonly AppState _appState;
        private readonly TeamService _teamService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TeamDto> Teams { get; } = new();

        public TeamsViewModel(AppState appState, TeamService teamService) {
            _appState = appState;
            _teamService = teamService;

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null) {
                await LoadTeams();
            } else {
                Teams.Clear();
            }
        }

        private async Task LoadTeams() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var leagueId = _appState.SelectedLeague.Id;
            var teams = await _teamService.GetTeamsAsync(leagueId);

            Teams.Clear();

            foreach (var team in teams) {
                Teams.Add(team);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null) {
                await LoadTeams();
            }
        }
    }
}