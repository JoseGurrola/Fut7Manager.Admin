using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
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

        public ObservableCollection<MatchdayDto> Matchdays { get; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public LeagueDto League { get; set; }

        //public ObservableCollection<MatchdayGroupDto> Matchdays { get; } = new();

        public MatchesViewModel(AppState appState, Fut7MatchService fut7MatchService, LeagueDto league) {
            _appState = appState;
            Fut7MatchService = fut7MatchService;
            League = league;

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            } else {
                Matchdays.Clear();
            }
        }

        private async Task LoadMatches() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var leagueId = _appState.SelectedLeague.Id;
            var matchdays = await Fut7MatchService.GetMatchdaysAsync(leagueId);

            Matchdays.Clear();

            foreach (var matchday in matchdays) {
                Matchdays.Add(matchday);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null) {
                await LoadMatches();
            }
        }

        public async Task UpdateMatch(Fut7MatchDetailsDto match) {
            await Fut7MatchService.UpdateFut7MatchAsync(match);

            // refrescar
            await LoadMatches();
        }
    }
}