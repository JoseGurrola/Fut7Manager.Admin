using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.Views;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class TeamListViewModel : BaseViewModel {
        private readonly TeamService _teamService;
        private readonly AppState _appState; // ViewModel principal para navegación
        private int _leagueId;

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

        //private TeamDto? _selectedTeam;
        //public TeamDto? SelectedTeam
        //{
        //    get => _selectedTeam;
        //    set {
        //        if (SetProperty(ref _selectedTeam, value)) {
        //            // Notifica a los comandos que puedan cambiar su estado (habilitado/deshabilitado)
        //            (OpenLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //            (EditLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //            (DeleteLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //        }
        //    }
        //}

        public ObservableCollection<TeamDto> Teams { get; } = new();

        public ICommand OpenTeamCommand { get; }
        public ICommand EditTeamCommand { get; }
        public ICommand DeleteTeamCommand { get; }
        public ICommand CreateTeamCommand { get; }

        public TeamListViewModel(AppState appState, TeamService teamService, int leagueId) {
            _appState = appState;
            _teamService = teamService;
            _leagueId = leagueId;

            OpenTeamCommand = new RelayCommand(OpenTeam, CanModifyTeam);
            EditTeamCommand = new RelayCommand(async () => await EditTeamAsync(), CanModifyTeam);
            DeleteTeamCommand = new RelayCommand(async () => await DeleteTeamAsync(), CanModifyTeam);
            CreateTeamCommand = new RelayCommand(async () => await CreateTeamAsync(), () => true);


            //main.LeagueChanged += OnLeagueChanged;
        }

        private bool CanModifyTeam() => SelectedTeam != null;

        //private async void OnLeagueChanged() {
        //    if (_appState.SelectedLeague != null) {
        //        await LoadTeams();
        //    } else {
        //        Teams.Clear();
        //    }
        //}

        private async Task LoadTeams() {
            //if(SelectedTeam == null) return;

            IsLoading = true;

            var teams = await _teamService.GetTeamsAsync(_leagueId);

            Teams.Clear();

            foreach (var team in teams) {
                Teams.Add(team);
            }

            IsLoading = false;
        }

        public async Task InitializeAsync() {
            //if ( != null) {
                await LoadTeams();
            //}
        }

        private void OpenTeam() {
            if (SelectedTeam == null) return;

            //_main.SelectedTeam(SelectedLeague);
            //TODO: al seleccionar que hacer, abrir vista de detalles del equipo o algo similar
            //var vm = new TeamListViewModel(_main.AppState, _main.TeamService);
           // _main.CurrentView = vm;
           // _ = vm.InitializeAsync();
        }

        private async Task EditTeamAsync() {
            if (SelectedTeam == null) return;

            var window = new CreateTeamWindow(); // Reusa ventana de crear liga
            var vm = new CreateOrEditTeamViewModel(SelectedTeam, SelectedTeam.LeagueId); // ViewModel unificado para crear/editar
            window.DataContext = vm;

            // Permite que el ViewModel cierre la ventana con DialogResult
            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Editar Equipo";

            var result = window.ShowDialog(); // Muestra la ventana como modal

            if (result == true) {

                var success = await _teamService.EditTeamAsync(
                    new TeamDto {
                        Id = SelectedTeam.Id,
                        Name = vm.TeamName,
                        LogoUrl = vm.LogoUrl,
                        GroupId = vm.SelectedGroup,
                        LeagueId = _leagueId
                    }
                );

                if (success) {
                    var index = Teams.IndexOf(SelectedTeam);

                    var updateTeam = new TeamDto {
                        Id = SelectedTeam.Id,
                        Name = vm.TeamName,
                        LogoUrl = vm.LogoUrl,
                        GroupId = vm.SelectedGroup,
                        LeagueId = vm.LeagueId
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
            var vm = new CreateOrEditTeamViewModel(null, Teams[0].LeagueId); // Null indica creación
            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Crear Equipo";

            var result = window.ShowDialog();
            if (result == true) {
                //System.Diagnostics.Debug.WriteLine($"Group selected by user: {vm.SelectedGroup}");
                var created = await _teamService.CreateTeamAsync(new TeamDto { 
                    Name = vm.TeamName, 
                    LogoUrl = vm.LogoUrl, 
                    GroupId = vm.SelectedGroup,
                    LeagueId = vm.LeagueId
                });
                if (created != null) {
                    Teams.Add(created); // Añade a la colección observable
                }
            }
        }
    }
}
