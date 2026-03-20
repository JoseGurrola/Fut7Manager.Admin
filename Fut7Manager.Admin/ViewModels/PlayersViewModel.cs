using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;

namespace Fut7Manager.Admin.ViewModels {
    public class PlayersViewModel : BaseViewModel {
        private readonly AppState _appState;
        private readonly PlayerService _playerService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PlayerDto> Players { get; } = new();

        public PlayersViewModel(AppState appState, PlayerService playerService) {
            _appState = appState;
            _playerService = playerService;

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null) {
                await LoadPlayers();
            } else {
                Players.Clear();
            }
        }

        private async Task LoadPlayers() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var leagueId = _appState.SelectedLeague.Id;
            var players = await _playerService.GetPlayersAsync(leagueId);

            Players.Clear();

            foreach (var player in players) {
                Players.Add(player);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null) {
                await LoadPlayers();
            }
        }
    }
}