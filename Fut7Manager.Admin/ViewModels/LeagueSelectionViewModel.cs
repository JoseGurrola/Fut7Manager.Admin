using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {

    // ViewModel para la selección de ligas, maneja CRUD y navegación
    public class LeagueSelectionViewModel : BaseViewModel {
        private readonly LeagueService _leagueService; // Servicio para llamadas a API
        private readonly GroupService _groupService;
        private readonly TeamService _teamService;
        private readonly Fut7MatchService _fut7MatchService;
        private readonly MainViewModel _main; // ViewModel principal para navegación
        private bool _openingLeague;
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // Colección observable de ligas para la UI
        public ObservableCollection<LeagueDto> Leagues { get; } = new ObservableCollection<LeagueDto>();

        // Liga actualmente seleccionada
        private LeagueDto? _selectedLeague;
        public LeagueDto? SelectedLeague
        {
            get => _selectedLeague;
            set {
                if (SetProperty(ref _selectedLeague, value)) {
                    // Notifica a los comandos que puedan cambiar su estado (habilitado/deshabilitado)
                    (OpenLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (EditLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Comandos para acciones de UI
        public ICommand OpenLeagueCommand { get; }
        public ICommand EditLeagueCommand { get; }
        public ICommand CreateLeagueCommand { get; }
        public ICommand DeleteLeagueCommand { get; }

        public LeagueSelectionViewModel(MainViewModel main, LeagueService leagueService, TeamService teamService, GroupService groupService, Fut7MatchService fut7MatchService) {
            _main = main;
            _leagueService = leagueService;
            _teamService = teamService;
            _groupService = groupService;
            _fut7MatchService = fut7MatchService;

            // Inicializa los comandos con sus métodos y condiciones de habilitación
            OpenLeagueCommand = new RelayCommand(OpenLeague, CanModifyLeague);
            EditLeagueCommand = new RelayCommand(async () => await EditLeagueAsync(), CanModifyLeague);
            CreateLeagueCommand = new RelayCommand(async () => await CreateLeagueAsync(), () => true);
            DeleteLeagueCommand = new RelayCommand(async () => await DeleteLeagueAsync(), CanModifyLeague);
        }

        // Valida si hay liga seleccionada
        private bool CanModifyLeague() => SelectedLeague != null;

        // Inicializa la lista de ligas desde la API
        public async Task InitializeAsync() {

            IsLoading = true;

            var leagues = await _leagueService.GetLeaguesAsync();
            Leagues.Clear();
            foreach (var league in leagues)
                Leagues.Add(league);

            IsLoading = false;
        }

        // Abre la vista central
        private async void OpenLeague() {

            if (_openingLeague)
                return;

            try {
                _openingLeague = true;
                if (SelectedLeague == null)
                    return;

                _main.SelectLeague(SelectedLeague);

                var vm = new CentralPanelViewModel(_main.AppState,_leagueService,SelectedLeague,
                    _teamService,_groupService,_fut7MatchService);

                _main.CurrentView = vm;

                await vm.InitializeAsync();
            }
            finally {

                _openingLeague = false;
            }
        }

        // Edita el nombre de la liga seleccionada
        private async Task EditLeagueAsync() {
            System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 1");
            if (SelectedLeague == null)
                return;

            var currentGroups =
                await _groupService.GetGroupsAsync(
                    SelectedLeague.Id);
            System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 2");
            var currentGroupCount =
                currentGroups.Count;
           
            var window = new CreateLeagueWindow();

            var vm = new CreateOrEditLeagueViewModel(SelectedLeague, currentGroupCount);

            window.DataContext = vm;

            vm.CloseAction =result => window.DialogResult = result;

            window.Title = "Editar liga";

            var result = window.ShowDialog();

            if (result != true) {
                System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 3");
                return;
            }

            // Siempre mínimo 1 grupo
            if (vm.NumberOfGroups <= 0)
                vm.NumberOfGroups = 1;

            // Solo tocar grupos si cambió la cantidad
            if (vm.NumberOfGroups != currentGroupCount) {
                System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 5");
                // Borrar grupos actuales
                foreach (var g in currentGroups) {
                    if (g.Id.HasValue) {
                        await _groupService.DeleteGroupAsync(
                            g.Id.Value);
                    }
                }
                System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 6");
                // Crear nuevos grupos
                for (var i = 0; i < vm.NumberOfGroups; i++) {

                    await _groupService.CreateGroupAsync(
                        new GroupDto {
                            Name = $"Grupo {i + 1}",
                            LeagueId = SelectedLeague.Id
                        });
                }

            }


            var success =
                await _leagueService.EditLeagueAsync(
                    new LeagueDto {
                        Id = SelectedLeague.Id,
                        Name = vm.LeagueName,
                        RegistrationFee = vm.RegistrationFee,
                        UsePenaltyShootoutPoints = vm.UsePenaltyShootoutPoints,
                        QualifiedTeamsPerGroup = vm.QualifiedTeamsPerGroup,
                        Status = vm.Status,
                        LogoUrl = vm.FinalLogoUrl
                    });

            if (!success) {
                System.Diagnostics.Debug.WriteLine("[EditLeagueAsync] 4");
                return;
            }

            var index = Leagues.IndexOf(SelectedLeague);

            var updatedLeague =
                new LeagueDto {
                    Id = SelectedLeague.Id,
                    Name = vm.LeagueName,
                    RegistrationFee = vm.RegistrationFee,
                    UsePenaltyShootoutPoints = vm.UsePenaltyShootoutPoints,
                    QualifiedTeamsPerGroup = vm.QualifiedTeamsPerGroup,
                    Status = vm.Status,
                    LogoUrl = vm.FinalLogoUrl
                };

            Leagues[index] = updatedLeague;

            SelectedLeague = updatedLeague;

           
        }

        // Crea una nueva liga
        private async Task CreateLeagueAsync() {
            var window = new CreateLeagueWindow();
            var vm = new CreateOrEditLeagueViewModel(null, null); // Null indica creación
            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Crear liga";

            var result = window.ShowDialog();
            if (result == true) {

                var created = await _leagueService.CreateLeagueAsync(new LeagueDto {
                    Name = vm.LeagueName,
                    RegistrationFee = vm.RegistrationFee,
                    UsePenaltyShootoutPoints = vm.UsePenaltyShootoutPoints,
                    QualifiedTeamsPerGroup = vm.QualifiedTeamsPerGroup,
                    LogoUrl = vm.FinalLogoUrl
                });
                if (created != null) {
                    if (vm.NumberOfGroups == 0) vm.NumberOfGroups = 1;
                    for (var i = 0; i < vm.NumberOfGroups; i++) {
                        await _groupService.CreateGroupAsync(new GroupDto { Name = $"Grupo {i + 1}", LeagueId = created.Id });
                    }
                    Leagues.Add(created); // Añade a la colección observable
                }
            }
        }

        // Elimina la liga seleccionada
        private async Task DeleteLeagueAsync() {
            if (SelectedLeague == null) return;

            var leagueToDelete = SelectedLeague;

            // Muestra un diálogo de confirmación
            var dialog = new ConfirmDialog();
            var vm = new ConfirmDialogViewModel($"¿Seguro que deseas eliminar la liga '{leagueToDelete.Name}'?");
            dialog.DataContext = vm;

            var result = dialog.ShowDialog();
            if (result != true) return;

            var success = await _leagueService.DeleteLeagueAsync(leagueToDelete.Id);
            if (success) {
                Leagues.Remove(leagueToDelete);
                SelectedLeague = null;
            }
        }
    }
}