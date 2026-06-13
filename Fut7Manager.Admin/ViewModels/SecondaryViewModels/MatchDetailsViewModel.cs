using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {
    public class MatchDetailsViewModel : BaseViewModel {
        private readonly PlayerService _playerService;
        private readonly Fut7MatchDetailsDto _match;

        public int HomeGoalsCaptured => HomePlayers.Sum(x => x.Goals);

        public int AwayGoalsCaptured => AwayPlayers.Sum(x => x.Goals);
        public ObservableCollection<MatchPlayerStatDto> HomePlayers { get; } = new();

        public ObservableCollection<MatchPlayerStatDto> AwayPlayers { get; } = new();

        public string HomeTeamName { get; }

        //public string? HomeTeamLogo { get; }

        public string AwayTeamName { get; }

        public int HomeGoalsExpected => _match.HomeGoals ?? 0;

        public int AwayGoalsExpected => _match.AwayGoals ?? 0;

        public bool HomeGoalsMatch => HomeGoalsCaptured == HomeGoalsExpected;

        public bool AwayGoalsMatch => AwayGoalsCaptured == AwayGoalsExpected;

        public bool HomeGoalsDbMatch =>
    (_match.HomePlayerStats?.Sum(x => x.Goals) ?? 0) == (_match.HomeGoals ?? 0);

        public bool AwayGoalsDbMatch =>
            (_match.AwayPlayerStats?.Sum(x => x.Goals) ?? 0) == (_match.AwayGoals ?? 0);

        public bool CanSaveDetails
        {
            get {
                // goles esperados según marcador del partido
                var expectedHome = _match.HomeGoals ?? 0;
                var expectedAway = _match.AwayGoals ?? 0;

                // goles capturados en la UI
                var capturedHome = HomePlayers.Sum(x => x.Goals);
                var capturedAway = AwayPlayers.Sum(x => x.Goals);

                // goles que venían de la BD
                var dbHome = _match.HomePlayerStats?.Sum(x => x.Goals) ?? 0;
                var dbAway = _match.AwayPlayerStats?.Sum(x => x.Goals) ?? 0;

                // válido si coincide con marcador usando UI o BD
                return (capturedHome == expectedHome || dbHome == expectedHome)
                    && (capturedAway == expectedAway || dbAway == expectedAway);
            }
        }


        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public Action<bool>? CloseAction { get; set; }

        public MatchDetailsViewModel(Fut7MatchDetailsDto match,PlayerService playerService) {
            _match = match;
            _playerService = playerService;

            HomeTeamName = match.HomeTeamName;
            AwayTeamName = match.AwayTeamName;
            //HomeTeamLogo = match.HomeTeamLogo;

            SaveCommand = new RelayCommand(async () =>  await Save());

            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));

            //_ = LoadPlayers();
        }

        public async Task InitializeAsync() {
            await LoadPlayers(); // ahora se llama desde fuera
        }

        private async Task Save() {
            if (!CanSaveDetails) {
                MessageService.Show("Los goles capturados no coinciden con el marcador del partido.");
                return;
            }

            // 🔹 HOME TEAM
            _match.HomePlayerStats = HomePlayers
                .Where(x =>
                    x.PlayerId.HasValue &&                // debe tener jugador asignado
                    !string.IsNullOrWhiteSpace(x.PlayerName) &&
                    (x.Goals > 0 || x.YellowCards > 0 || x.RedCards > 0))
                .GroupBy(x => x.PlayerId)                // evita duplicados
                .Select(g => g.First())                  // toma el primero si hay repetidos
                .ToList();

            // 🔹 AWAY TEAM
            _match.AwayPlayerStats = AwayPlayers
                .Where(x =>
                    x.PlayerId.HasValue &&
                    !string.IsNullOrWhiteSpace(x.PlayerName) &&
                    (x.Goals > 0 || x.YellowCards > 0 || x.RedCards > 0))
                .GroupBy(x => x.PlayerId)
                .Select(g => g.First())
                .ToList();

            CloseAction?.Invoke(true);
        }


        private async Task LoadPlayers() {
            try {
                // 🔹 HOME TEAM
                var homePlayers = await _playerService.GetPlayersByTeamAsync(_match.HomeTeamId);
                HomePlayers.Clear();

                foreach (var player in homePlayers) {
                    // busca si ya hay stats para este jugador
                    var stat = _match.HomePlayerStats
                        ?.FirstOrDefault(s => s.PlayerId == player.Id);

                    var item = stat ?? new MatchPlayerStatDto {
                        PlayerId = player.Id,
                        PlayerName = player.Name,
                        JerseyNumber = player.JerseyNumber
                    };

                    item.PropertyChanged += PlayerStat_PropertyChanged;
                    HomePlayers.Add(item);
                }

                // agrega filas vacías para capturar jugadores adicionales
                for (int i = 0; i < 3; i++) {
                    var item = new MatchPlayerStatDto();
                    item.PropertyChanged += PlayerStat_PropertyChanged;
                    HomePlayers.Add(item);
                }

                // 🔹 AWAY TEAM
                var awayPlayers = await _playerService.GetPlayersByTeamAsync(_match.AwayTeamId);
                AwayPlayers.Clear();

                foreach (var player in awayPlayers) {
                    var stat = _match.AwayPlayerStats
                        ?.FirstOrDefault(s => s.PlayerId == player.Id);

                    var item = stat ?? new MatchPlayerStatDto {
                        PlayerId = player.Id,
                        PlayerName = player.Name,
                        JerseyNumber = player.JerseyNumber
                    };

                    item.PropertyChanged += PlayerStat_PropertyChanged;
                    AwayPlayers.Add(item);
                }

                for (int i = 0; i < 3; i++) {
                    var item = new MatchPlayerStatDto();
                    item.PropertyChanged += PlayerStat_PropertyChanged;
                    AwayPlayers.Add(item);
                }

                OnPropertyChanged(nameof(HomeGoalsCaptured));
                OnPropertyChanged(nameof(AwayGoalsCaptured));
                OnPropertyChanged(nameof(HomeGoalsMatch));
                OnPropertyChanged(nameof(AwayGoalsMatch));
                OnPropertyChanged(nameof(HomeGoalsDbMatch));
                OnPropertyChanged(nameof(AwayGoalsDbMatch));
                OnPropertyChanged(nameof(CanSaveDetails));
            }
            catch (Exception ex) {
                MessageService.Show(ex.ToString());
            }
        }


        private void PlayerStat_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(MatchPlayerStatDto.Goals)) {
                OnPropertyChanged(nameof(HomeGoalsCaptured));
                OnPropertyChanged(nameof(AwayGoalsCaptured));

                OnPropertyChanged(nameof(HomeGoalsMatch));
                OnPropertyChanged(nameof(AwayGoalsMatch));

                // 🔥 recalcular validaciones que usan datos de BD
                OnPropertyChanged(nameof(HomeGoalsDbMatch));
                OnPropertyChanged(nameof(AwayGoalsDbMatch));

                OnPropertyChanged(nameof(CanSaveDetails));
            }
        }

    }
}
