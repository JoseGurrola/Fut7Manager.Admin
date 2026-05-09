using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Fut7Manager.Admin.ViewModels {
    public class PlayersViewModel : BaseViewModel {

        private readonly AppState _appState;
        private readonly PlayerService _playerService;
        private Color _teamPrimaryColor;

        public ObservableCollection<PlayerDto> Players { get; } = new();

        private List<PlayerDto> _allPlayers = new();

        public ObservableCollection<string> TeamFilters { get; } = new();

        public ObservableCollection<string> StatusFilters { get; } =
            new() { "Todos", "Activos", "Inactivos" };

        private PlayerDto? _selectedPlayer;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private string _selectedTeamFilter = "Todos";
        private string _selectedStatusFilter = "Todos";

        public PlayerDto? SelectedPlayer
        {
            get => _selectedPlayer;
            set {
                _selectedPlayer = value;
                OnPropertyChanged();

                (EditPlayerCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeletePlayerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public Color TeamPrimaryColor
        {
            get => _teamPrimaryColor;
            set { _teamPrimaryColor = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string SelectedTeamFilter
        {
            get => _selectedTeamFilter;
            set {
                _selectedTeamFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set {
                _selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ICommand CreatePlayerCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand DeletePlayerCommand { get; }

        public PlayersViewModel(
            AppState appState,
            PlayerService playerService) {

            _appState = appState;
            _playerService = playerService;

            CreatePlayerCommand =
                new RelayCommand(async () => await CreatePlayerAsync());

            EditPlayerCommand =
                new RelayCommand(async () => await EditPlayer());

            DeletePlayerCommand =
                new RelayCommand(async () => await DeletePlayer());

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {

            if (_appState.SelectedLeague != null)
                await LoadPlayers();
            else
                Players.Clear();
        }

        public async Task InitializeAsync() {

            if (_appState.SelectedLeague != null)
                await LoadPlayers();
        }

        private async Task LoadPlayers() {
            if (_appState.SelectedLeague == null)
                return;

            try {
                IsLoading = true;

                var players =
                    await _playerService.GetPlayersAsync(
                        _appState.SelectedLeague.Id);

                _allPlayers = players ?? new List<PlayerDto>();

                SelectedPlayer = null;

                LoadTeamFilters();

                // Validar filtro
                if (!TeamFilters.Contains(SelectedTeamFilter))
                    _selectedTeamFilter = "Todos";

                ApplyFilters();
            }
            finally {
                IsLoading = false;
            }
        }

        private void LoadTeamFilters() {

            TeamFilters.Clear();

            TeamFilters.Add("Todos");

            var teams = _allPlayers
                .Select(x => x.TeamName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x);

            foreach (var team in teams)
                TeamFilters.Add(team);

        }

        private void ApplyFilters() {

            IEnumerable<PlayerDto> filtered = _allPlayers;

            // búsqueda
            if (!string.IsNullOrWhiteSpace(SearchText)) {

                var search = SearchText.ToLower();

                filtered = filtered.Where(p =>

                    (!string.IsNullOrWhiteSpace(p.Name) &&
                     p.Name.ToLower().Contains(search))

                    ||

                    (!string.IsNullOrWhiteSpace(p.Phone) &&
                     p.Phone.ToLower().Contains(search))

                    ||

                    (!string.IsNullOrWhiteSpace(p.Email) &&
                     p.Email.ToLower().Contains(search)));
            }

            // equipo
            if (SelectedTeamFilter != "Todos") {
                filtered = filtered.Where(p =>
                    !string.IsNullOrWhiteSpace(p.TeamName) &&
                    p.TeamName == SelectedTeamFilter);
            }

            // estado
            if (SelectedStatusFilter == "Activos") {

                filtered = filtered.Where(p => p.Active);
            } else if (SelectedStatusFilter == "Inactivos") {

                filtered = filtered.Where(p => !p.Active);
            }

            Players.Clear();

            foreach (var player in filtered)
                Players.Add(player);
        }

        private async Task CreatePlayerAsync() {

            if (_appState.SelectedLeague == null)
                return;

            var window = new CreatePlayerWindow();

            var vm = new CreateOrEditPlayerViewModel(
                _appState.SelectedLeague);

            window.DataContext = vm;

            vm.CloseAction = result =>
                window.DialogResult = result;

            var result = window.ShowDialog();

            if (result == true) {

                var created =
                    await _playerService.CreatePlayerAsync(
                        new PlayerDto {
                            Name = vm.Name,
                            JerseyNumber = vm.JerseyNumber,
                            Phone = vm.Phone,
                            Email = vm.Email,
                            Position = vm.Position,
                            Active = vm.Active,
                            TeamId = vm.SelectedTeamId,
                            DateOfBirth = vm.DateOfBirth ?? DateTime.MinValue
                        });

                if (created != null)
                    await LoadPlayers();
            }
        }

        private async Task EditPlayer() {

            if (SelectedPlayer == null)
                return;

            if (_appState.SelectedLeague == null)
                return;

            var window = new CreatePlayerWindow();

            var vm = new CreateOrEditPlayerViewModel(
                _appState.SelectedLeague,
                SelectedPlayer);

            window.DataContext = vm;

            vm.CloseAction = result =>
                window.DialogResult = result;

            var result = window.ShowDialog();

            if (result == true) {

                var updated =
                    await _playerService.EditPlayerAsync(
                        SelectedPlayer.Id,
                        new PlayerDto {
                            Id = SelectedPlayer.Id,
                            Name = vm.Name,
                            JerseyNumber = vm.JerseyNumber,
                            Phone = vm.Phone,
                            Email = vm.Email,
                            Position = vm.Position,
                            Active = vm.Active,
                            TeamId = vm.SelectedTeamId,
                            DateOfBirth = vm.DateOfBirth ?? DateTime.MinValue
                        });

                if (updated)
                    await LoadPlayers();
            }
        }

        private async Task DeletePlayer() {

            if (SelectedPlayer == null)
                return;

            var result = MessageBox.Show(
                $"¿Eliminar a {SelectedPlayer.Name}?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var success =
                await _playerService.DeletePlayerAsync(
                    SelectedPlayer.Id);

            if (success)
                await LoadPlayers();
        }
    }
}