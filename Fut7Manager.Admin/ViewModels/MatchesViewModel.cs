using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;

namespace Fut7Manager.Admin.ViewModels {
    public class MatchdayGroupDto {
        public int MatchdayId { get; set; }
        public string MatchdayName { get; set; } = "";
        public ObservableCollection<Fut7MatchDto> Matches { get; set; } = new();
    }

    public class MatchesViewModel : BaseViewModel {
        private readonly AppState _appState;
        public Fut7MatchService Fut7MatchService { get; }

        public ObservableCollection<Fut7MatchDto> AllMatches { get; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        //public ObservableCollection<MatchdayGroupDto> Matchdays { get; } = new();

        public MatchesViewModel(AppState appState, Fut7MatchService fut7MatchService) {
            _appState = appState;
            Fut7MatchService = fut7MatchService;

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            } else {
                AllMatches.Clear();
            }
        }

        private async Task LoadMatches() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var leagueId = _appState.SelectedLeague.Id;
            var matches = await Fut7MatchService.GetFut7MatchsAsync(leagueId);

            AllMatches.Clear();

            foreach (var match in matches) {
                AllMatches.Add(match);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            }
        }

        public async Task UpdateMatch(Fut7MatchDto match) {
            await Fut7MatchService.UpdateFut7MatchAsync(match);

            // refrescar
            await LoadMatches();
        }
    }
}