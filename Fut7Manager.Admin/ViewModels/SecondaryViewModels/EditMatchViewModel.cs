using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels.SecondaryViewModels {
    public class EditMatchViewModel : BaseViewModel {
        private readonly Fut7MatchService _fut7MatchService;
        private readonly Fut7MatchDto _match;
        private int? _homeGoals;
        private int? _awayGoals;
        private int? _homePenaltyGoals;
        private int? _awayPenaltyGoals;
        public Action<bool>? CloseAction { get; set; }

        public string MatchTitle => $"{_match.HomeTeamName} vs {_match.AwayTeamName}";

        // =========================
        // PROPIEDADES REACTIVAS
        // =========================

        private DateTime? _matchDate;
        public DateTime? MatchDate
        {
            get => _matchDate;
            set {
                _matchDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullDatePreview));
            }
        }

        private int _selectedHour;
        public int SelectedHour
        {
            get => _selectedHour;
            set {
                _selectedHour = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullDatePreview));
            }
        }

        private int _selectedMinute;
        public int SelectedMinute
        {
            get => _selectedMinute;
            set {
                _selectedMinute = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullDatePreview));
            }
        }

        public string? Location { get; set; }
        public int? HomeGoals
        {
            get => _homeGoals;
            set {
                _homeGoals = value;

                // si ya no es empate, limpiar penales
                if (_homeGoals != _awayGoals) {
                    HomePenaltyGoals = null;
                    AwayPenaltyGoals = null;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditPenalties));
            }
        }

        public int? AwayGoals
        {
            get => _awayGoals;
            set {
                _awayGoals = value;

                // si ya no es empate, limpiar penales
                if (_homeGoals != _awayGoals) {
                    HomePenaltyGoals = null;
                    AwayPenaltyGoals = null;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditPenalties));
            }
        }
        
        public int? HomePenaltyGoals
        {
            get => _homePenaltyGoals;
            set {
                _homePenaltyGoals = value;
                OnPropertyChanged();
            }
        }

       
        public int? AwayPenaltyGoals
        {
            get => _awayPenaltyGoals;
            set {
                _awayPenaltyGoals = value;
                OnPropertyChanged();
            }
        }

        public LeagueDto League { get;}
        public bool CanEditPenalties =>
            League.UsePenaltyShootoutPoints &&
            HomeGoals.HasValue &&
            AwayGoals.HasValue &&
            HomeGoals == AwayGoals;

        // =========================
        // LISTAS
        // =========================

        public List<int> Hours { get; } = Enumerable.Range(7, 16).ToList(); // 7 a 22
        public List<int> Minutes { get; } = new() { 0, 10, 20, 30, 40, 50 };

        // =========================
        // PREVIEW
        // =========================

        public string FullDatePreview =>
            MatchDate == null
                ? ""
                : $"{MatchDate:dd/MM/yyyy} {SelectedHour:D2}:{SelectedMinute:D2}";

        // =========================
        // COMMANDS
        // =========================

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public EditMatchViewModel(Fut7MatchDto match, Fut7MatchService fut7MatchService, LeagueDto league) {
            _match = match;
            _fut7MatchService = fut7MatchService;

            League = league;
            MatchDate = match.MatchDate ?? DateTime.Today;
            Location = match.Location;
            HomeGoals = match.HomeGoals;
            AwayGoals = match.AwayGoals;
            HomePenaltyGoals = match.HomePenaltyGoals;
            AwayPenaltyGoals = match.AwayPenaltyGoals;

            // 🔥 Inicializar hora/minuto correctamente
            if (match.MatchDate.HasValue) {
                SelectedHour = match.MatchDate.Value.Hour;
                SelectedMinute = (match.MatchDate.Value.Minute / 10) * 10; // normaliza
            } else {
                SelectedHour = 19;
                SelectedMinute = 0;
            }

            SaveCommand = new RelayCommand(async () => await Save());
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }

        // =========================
        // SAVE
        // =========================

        private async Task Save() {

            if (MatchDate == null) {

                MessageService.Show("Selecciona una fecha");
                return;
            }

            if (HomeGoals < 0 || AwayGoals < 0) {

                MessageService.Show("El marcador no puede ser negativo");
                return;
            }

            if (CanEditPenalties) {
                if (!HomePenaltyGoals.HasValue ||
                    !AwayPenaltyGoals.HasValue) {
                    MessageService.Show("Captura el marcador de penales");
                    return;
                }

                if (HomePenaltyGoals == AwayPenaltyGoals) {
                    MessageService.Show("Los penales no pueden terminar empatados");
                    return;
                }
            }

            _match.MatchDate = BuildMatchDateTime();
            _match.Location = Location;
            _match.HomeGoals = HomeGoals;
            _match.AwayGoals = AwayGoals;
            _match.HomePenaltyGoals = HomePenaltyGoals;
            _match.AwayPenaltyGoals = AwayPenaltyGoals;

            var success = await _fut7MatchService.UpdateFut7MatchAsync(_match);

            if (success) {

                CloseAction?.Invoke(true);

            } else {

                MessageService.Show(
                    "Error al guardar partido",
                    "Error");
            }
        }

        private DateTime? BuildMatchDateTime() {
            if (MatchDate == null)
                return null;

            return new DateTime(
                MatchDate.Value.Year,
                MatchDate.Value.Month,
                MatchDate.Value.Day,
                SelectedHour,
                SelectedMinute,
                0
            );
        }
    }
}