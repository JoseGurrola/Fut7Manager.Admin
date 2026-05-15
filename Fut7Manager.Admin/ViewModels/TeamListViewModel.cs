using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class TeamListViewModel : BaseViewModel {
        private readonly TeamService _teamService;
        private readonly AppState _appState; // ViewModel principal para navegación
        //private int _leagueId;
        private readonly MainViewModel _mainViewModel;

        private LeagueDto _league;
        private string _sortColumn = "Name";
        private bool _sortAscending = true;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private TeamDto? _selectedTeam;
        public TeamDto? SelectedTeam
        {
            get => _selectedTeam;
            set { _selectedTeam = value; OnPropertyChanged(); }
        }

        public LeagueDto League
        {
            get => _league;
            set {
                _league = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditTeamInfo));

                (CreateTeamCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string CreateTeamTooltip => CanEditTeamInfo
                ? "Agregar equipo"
                : "No se pueden agregar equipos porque la liga ya inició";

        public string DeleteTeamTooltip => CanEditTeamInfo
                ? "Agregar equipo"
                : "No se pueden eliminar equipos porque la liga ya inició";

        public bool CanEditTeamInfo => _league.Status == LeagueStatus.Upcoming;

        public string SortColumn
        {
            get => _sortColumn;
            set {
                _sortColumn = value;
                OnPropertyChanged();
            }
        }

        public bool SortAscending
        {
            get => _sortAscending;
            set {
                _sortAscending = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SortDirectionSymbol));
            }
        }

        public string SortDirectionSymbol =>
            SortAscending ? "↑" : "↓";

        public ObservableCollection<TeamDto> Teams { get; } = new();

        public ICommand OpenTeamCommand { get; }
        public ICommand EditTeamCommand { get; }
        public ICommand DeleteTeamCommand { get; }
        public ICommand CreateTeamCommand { get; }

        public ICommand OpenPaymentsCommand { get; }

        public ICommand SortCommand { get; }
        public TeamListViewModel(MainViewModel mainViewModel, AppState appState, TeamService teamService, LeagueDto league) {
            _mainViewModel = mainViewModel;
            _appState = appState;
            _teamService = teamService;
            _league = league;

            OpenTeamCommand = new RelayCommand(OpenTeam, CanModifyTeam);
            EditTeamCommand = new RelayCommand(async () => await EditTeamAsync(), CanModifyTeam);
            DeleteTeamCommand = new RelayCommand(async () => await DeleteTeamAsync(), CanModifyTeam);
            CreateTeamCommand = new RelayCommand(async () => await CreateTeamAsync(), () => CanEditTeamInfo);
            OpenPaymentsCommand = new RelayCommand(OpenPayments, CanModifyTeam);
            SortCommand = new RelayCommand<string>(SortBy);
            //main.LeagueChanged += OnLeagueChanged;
        }


        private bool CanModifyTeam() => SelectedTeam != null;

        private async Task LoadTeams() {
            //if(SelectedTeam == null) return;

            IsLoading = true;

            var teams = await _teamService.GetTeamsAsync(_league.Id);

            Teams.Clear();

            foreach (var team in teams) {
                Teams.Add(team);
            }

            ApplySorting();

            IsLoading = false;
        }

        private void OpenPayments() {
            if (SelectedTeam == null) return;

            var window = new PaymentsWindow();
            var vm = new PaymentsViewModel(
                new PaymentService(),
                SelectedTeam.Id,
                SelectedTeam.Name);

            window.DataContext = vm;

            vm.CloseAction = result =>
            {
                window.DialogResult = result;
                window.Close();
            };

            window.ShowDialog();

            _ = LoadTeams(); // refrescar estado de pagos

        }

        public async Task InitializeAsync() {
            //if ( != null) {
                await LoadTeams();
            //}
        }

        private async void OpenTeam() {

            if (SelectedTeam == null)
                return;

            // guardar filtro pendiente
            _appState.PendingPlayerTeamFilter =
                SelectedTeam.Name;

            // navegar
            var vm = new PlayersViewModel(
                _appState,
                _mainViewModel.PlayerService);

            _mainViewModel.CurrentView = vm;

            await vm.InitializeAsync();
        }

        private async Task EditTeamAsync() {
            if (SelectedTeam == null) return;

            var window = new CreateTeamWindow(); // Reusa ventana de crear liga
            var vm = new CreateOrEditTeamViewModel(SelectedTeam, _league); // ViewModel unificado para crear/editar
            window.DataContext = vm;

            // Permite que el ViewModel cierre la ventana con DialogResult
            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Editar Equipo";

            var result = window.ShowDialog(); // Muestra la ventana como modal

            if (result == true) {

                var success = await _teamService.EditTeamAsync(new TeamDto {
                        Id = SelectedTeam.Id,
                        Name = vm.TeamName,
                        LogoUrl = vm.LogoUrl,
                        GroupId = SelectedTeam.GroupId,
                        LeagueId = _league.Id,
                        Paid = vm.Paid,
                        Remaining = vm.Remaining,

                        TeamManagerName = vm.TeamManager,
                        TeamManagerPhone = vm.TeamManagerPhone,
                        TeamPrimaryColor = vm.TeamPrimaryColor.ToString()
                    }
                );

                if (success) {
                    var index = Teams.IndexOf(SelectedTeam);

                    var updateTeam = new TeamDto {
                        Id = SelectedTeam.Id,
                        Name = vm.TeamName,
                        LogoUrl = vm.LogoUrl,
                        GroupId = SelectedTeam.GroupId,
                        LeagueId = _league.Id,
                        Paid = vm.Paid,
                        Remaining = vm.Remaining,
                        TeamManagerName = vm.TeamManager,
                        TeamManagerPhone = vm.TeamManagerPhone,
                        TeamPrimaryColor = vm.TeamPrimaryColor.ToString()
                    };

                    Teams[index] = updateTeam;
                    SelectedTeam = updateTeam;
                }
            }
        }

        private async Task DeleteTeamAsync() {
            if (SelectedTeam == null) return;

            var teamToDelete = SelectedTeam;

            // Muestra un diálogo de confirmación
            var dialog = new ConfirmDialog();
            var vm = new ConfirmDialogViewModel($"¿Seguro que deseas eliminar el equipo '{teamToDelete.Name}'?");
            dialog.DataContext = vm;

            var result = dialog.ShowDialog();
            if (result != true) return;

            var success = await _teamService.DeleteTeamAsync(teamToDelete.Id);
            if (success) {
                Teams.Remove(teamToDelete);
                SelectedTeam = null;
            }
        }

        private async Task CreateTeamAsync() {
            var window = new CreateTeamWindow();
            var vm = new CreateOrEditTeamViewModel(null, _league); // Null indica creación
            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Crear Equipo";

            var result = window.ShowDialog();
            if (result == true) {
                //System.Diagnostics.Debug.WriteLine($"Group selected by user: {vm.SelectedGroup}");
                var created = await _teamService.CreateTeamAsync(new TeamDto { 
                    Name = vm.TeamName, 
                    LogoUrl = vm.LogoUrl, 
                    //GroupId = vm.SelectedGroup,
                    LeagueId = _league.Id,
                    Paid = vm.Paid,
                    Remaining = vm.Remaining,
                    TeamManagerName = vm.TeamManager,
                    TeamManagerPhone = vm.TeamManagerPhone,
                    TeamPrimaryColor = vm.TeamPrimaryColor.ToString()

                });
                if (created != null) {
                    Teams.Add(created); // Añade a la colección observable
                }
            }
        }

        private void SortBy(string? column) {
            if (string.IsNullOrWhiteSpace(column))
                return;

            if (SortColumn == column) {
                SortAscending = !SortAscending;
            } else {
                SortColumn = column;
                SortAscending = true;
            }

            ApplySorting();
        }

        private void ApplySorting() {

            IEnumerable<TeamDto> sorted = Teams;

            switch (SortColumn) {

                case "Name":

                sorted = SortAscending
                    ? Teams.OrderBy(t => t.Name)
                    : Teams.OrderByDescending(t => t.Name);

                break;

                case "PaymentStatus":

                sorted = SortAscending
                    ? Teams.OrderBy(t => GetPaymentOrder(t.PaymentStatus))
                    : Teams.OrderByDescending(t => GetPaymentOrder(t.PaymentStatus));

                break;
            }

            // MATERIALIZAR antes de Clear()
            var sortedList = sorted.ToList();

            var currentSelection = SelectedTeam;

            Teams.Clear();

            foreach (var team in sortedList)
                Teams.Add(team);

            SelectedTeam = currentSelection;
        }

        private int GetPaymentOrder(string status) {
            switch (status) {

                case "Paid":
                return 0;

                case "Partial":
                return 1;

                case "Due":
                return 2;

                default:
                return 99;
            }
        }
    }
}
