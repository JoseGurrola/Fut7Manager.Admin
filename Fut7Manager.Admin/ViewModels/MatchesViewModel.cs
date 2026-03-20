using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;

namespace Fut7Manager.Admin.ViewModels {
    public class MatchesViewModel : BaseViewModel {
        private readonly AppState _appState;
        private readonly MatchService _matchService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Fut7MatchDto> Matches { get; } = new();

        public MatchesViewModel(AppState appState, MatchService matchService) {
            _appState = appState;
            _matchService = matchService;

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            } else {
                Matches.Clear();
            }
        }

        private async Task LoadMatches() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var leagueId = _appState.SelectedLeague.Id;
            var matches = await _matchService.GetMatchesAsync(leagueId);

            Matches.Clear();

            foreach (var match in matches) {
                Matches.Add(match);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            }
        }
    }
}