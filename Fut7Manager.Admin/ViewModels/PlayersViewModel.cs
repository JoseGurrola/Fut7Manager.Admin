using ClosedXML.Excel;
using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views.SecondaryWindows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Fut7Manager.Admin.ViewModels {
    public class PlayersViewModel : BaseViewModel {
        private static string filterDefault = "Todos los equipos";
        private readonly AppState _appState;
        private readonly PlayerService _playerService;
        private Color _teamPrimaryColor;
        private string _sortColumn = "Name";
        private bool _sortAscending = true;
        private List<TeamDto> _teams = new();
        private readonly TeamService _teamService = new();
        public ObservableCollection<PlayerDto> Players { get; } = new();

        private List<PlayerDto> _allPlayers = new();

        public ObservableCollection<string> TeamFilters { get; } = new();

        public ObservableCollection<string> StatusFilters { get; } =
            new() { "Todos", "Activos", "Inactivos" };

        private PlayerDto? _selectedPlayer;
        private bool _isLoading;
        private string _searchText = string.Empty;
        private string _selectedTeamFilter = filterDefault;
        private string _selectedStatusFilter = "Activos";
        public int TotalPlayers => Players.Count;
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

        private TeamDto? GetSelectedTeam() {

            return _teams.FirstOrDefault(t =>
                t.Name == SelectedTeamFilter);
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
                OnPropertyChanged(nameof(CanImportPlayers));

                (ImportPlayersCommand as RelayCommand)
                    ?.RaiseCanExecuteChanged();

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

        public string NameSortIcon => _sortColumn == "Name" ? (_sortAscending ? " ↑" : " ↓") : "";
        public string PositionSortIcon => _sortColumn == "Position" ? (_sortAscending ? " ↑" : " ↓") : "";
        public string TeamSortIcon => _sortColumn == "TeamName" ? (_sortAscending ? " ↑" : " ↓") : "";
        public string NumberSortIcon => _sortColumn == "JerseyNumber" ? (_sortAscending ? " ↑" : " ↓") : "";
        public string StatusSortIcon => _sortColumn == "Active" ? (_sortAscending ? " ↑" : " ↓") : "";
        public bool CanImportPlayers => SelectedTeamFilter != filterDefault;

        public ICommand CreatePlayerCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand DeletePlayerCommand { get; }
        public ICommand ImportPlayersCommand { get; }
        public ICommand SortCommand { get; }
        public PlayersViewModel(AppState appState, PlayerService playerService) {

            _appState = appState;
            _playerService = playerService;

            Players.CollectionChanged += (_, __) => {
                OnPropertyChanged(nameof(TotalPlayers));
            };

            CreatePlayerCommand = new RelayCommand(async () => await CreatePlayerAsync());

            EditPlayerCommand = new RelayCommand(async () => await EditPlayer());

            DeletePlayerCommand = new RelayCommand(async () => await DeletePlayer());

            ImportPlayersCommand = new RelayCommand(async () => await ImportPlayers(), () => CanImportPlayers);

            SortCommand = new RelayCommand<string>(SortBy);

            _appState.LeagueChanged += OnLeagueChanged;

            
        }

        private async Task ImportPlayers() {

            if (SelectedTeamFilter == filterDefault) {
                MessageService.Show("Selecciona un equipo");
                return;
            }

            var selectedTeam = GetSelectedTeam();

            if (selectedTeam == null) {
                MessageService.Show("Equipo inválido");
                return;
            }

            var dialog = new OpenFileDialog {
                Filter = "Excel Files|*.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            var playersToImport = new List<PlayerDto>();

            XLWorkbook workbook;
            try {
                workbook = new XLWorkbook(dialog.FileName);
            }
            catch (IOException) {
                MessageService.Show("El archivo Excel está abierto. Ciérralo antes de importarlo.");
                return;
            }
            using (workbook) {

                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows) {

                    if (string.IsNullOrWhiteSpace(row.Cell(1).GetString()))
                        continue;

                    int jerseyNumber = 0;

                    int.TryParse(row.Cell(2).GetString(), out jerseyNumber);

                    var player = new PlayerDto {
                        Name = row.Cell(1).GetString().Trim(),
                        JerseyNumber = jerseyNumber,
                        Phone = row.Cell(3).GetString(),
                        Email = row.Cell(4).GetString(),
                        Position = ParsePosition(row.Cell(5).GetString()),
                        Active = ParseActive(row.Cell(6).GetString()),
                        TeamId = selectedTeam.Id
                    };

                    if (string.IsNullOrWhiteSpace(player.Name))
                        continue;

                    var dobCell = row.Cell(7);

                    if (!dobCell.IsEmpty()) {

                        if (DateTime.TryParse(
                            dobCell.GetString(), out var dob)) {

                            player.DateOfBirth = dob;

                        } else if (
                            dobCell.DataType == XLDataType.DateTime) {

                            player.DateOfBirth = dobCell.GetDateTime();
                        }
                    }
                    playersToImport.Add(player);
                }

                if (!playersToImport.Any()) {
                    MessageService.Show("No se encontraron jugadores válidos");
                    return;
                }

                var confirm = MessageService.Confirm(
                    $"Esto reemplazará todos los jugadores del equipo '{selectedTeam.Name}'. ¿Continuar?",
                    "Importar jugadores");

                if (!confirm)
                    return;

                var success = await _playerService.ImportPlayersAsync(selectedTeam.Id, playersToImport);

                if (!success) {
                    MessageService.Show("Error al importar jugadores");
                    return;
                }

                await LoadPlayers();

                MessageService.Show($"{playersToImport.Count} jugadores importados correctamente");
            }
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

        private void SortBy(string? column) {
            if (string.IsNullOrWhiteSpace(column))
                return;

            if (_sortColumn == column) {
                _sortAscending = !_sortAscending;
            } else {
                _sortColumn = column;
                _sortAscending = true;
            }

            OnPropertyChanged(nameof(NameSortIcon));
            OnPropertyChanged(nameof(PositionSortIcon));
            OnPropertyChanged(nameof(TeamSortIcon));
            OnPropertyChanged(nameof(NumberSortIcon));
            OnPropertyChanged(nameof(StatusSortIcon));

            ApplyFilters();
        }
        private async Task LoadPlayers() {

            if (_appState.SelectedLeague == null)
                return;

            try {

                IsLoading = true;

                var currentTeamFilter = SelectedTeamFilter;

                _teams = await _teamService.GetTeamsAsync(
                    _appState.SelectedLeague.Id);

                var players =
                    await _playerService.GetPlayersAsync(
                        _appState.SelectedLeague.Id);

                _allPlayers = players ?? new List<PlayerDto>();

                SelectedPlayer = null;

                LoadTeamFilters();

                // filtro pendiente desde equipos
                if (!string.IsNullOrWhiteSpace(_appState.PendingPlayerTeamFilter)
                    && TeamFilters.Contains(_appState.PendingPlayerTeamFilter)) {

                    SelectedTeamFilter =
                        _appState.PendingPlayerTeamFilter;

                    _appState.PendingPlayerTeamFilter = null;
                } else {

                    // restaurar filtro previo
                    if (TeamFilters.Contains(currentTeamFilter))
                        SelectedTeamFilter = currentTeamFilter;
                    else
                        SelectedTeamFilter = filterDefault;
                }

                ApplyFilters();
            }
            finally {

                IsLoading = false;
            }
        }

        private void LoadTeamFilters() {

            TeamFilters.Clear();

            TeamFilters.Add(filterDefault);

            foreach (var team in _teams)
                TeamFilters.Add(team.Name);

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
            if (SelectedTeamFilter != filterDefault) {
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

            filtered = ApplySorting(filtered);

            Players.Clear();

            foreach (var player in filtered)
                Players.Add(player);
        }

        private IEnumerable<PlayerDto> ApplySorting(IEnumerable<PlayerDto> players) {

            switch (_sortColumn) {

                case "JerseyNumber":
                return _sortAscending
                    ? players.OrderBy(p => p.JerseyNumber)
                    : players.OrderByDescending(p => p.JerseyNumber);

                case "Name":
                return _sortAscending
                    ? players.OrderBy(p => p.Name)
                    : players.OrderByDescending(p => p.Name);

                case "Position":
                return _sortAscending
                    ? players.OrderBy(p => p.Position)
                    : players.OrderByDescending(p => p.Position);

                case "Phone":
                return _sortAscending
                    ? players.OrderBy(p => p.Phone)
                    : players.OrderByDescending(p => p.Phone);

                case "Email":
                return _sortAscending
                    ? players.OrderBy(p => p.Email)
                    : players.OrderByDescending(p => p.Email);

                case "TeamName":
                return _sortAscending
                    ? players.OrderBy(p => p.TeamName)
                    : players.OrderByDescending(p => p.TeamName);

                case "Active":
                return _sortAscending
                    ? players.OrderBy(p => p.Active)
                    : players.OrderByDescending(p => p.Active);

                default:
                return _sortAscending
                    ? players.OrderBy(p => p.Name)
                    : players.OrderByDescending(p => p.Name);
            }
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

            var result = MessageService.Confirm($"¿Eliminar a {SelectedPlayer.Name}?", "Confirmar");

            if (!result)
                return;

            var success =
                await _playerService.DeletePlayerAsync(
                    SelectedPlayer.Id);

            if (success)
                await LoadPlayers();
        }

        private PlayerPosition ParsePosition(string value) {

            value = value.Trim().ToLower();

            return value switch {

                "portero" => PlayerPosition.Goalkeeper,
                "defensa" => PlayerPosition.Defender,
                "medio" => PlayerPosition.Midfielder,
                "delantero" => PlayerPosition.Forward,

                _ => PlayerPosition.Goalkeeper
            };
        }

        private bool ParseActive(string value) {

            value = value.Trim().ToLower();

            return value == "si"
                || value == "sí"
                || value == "true";
        }
    }
}