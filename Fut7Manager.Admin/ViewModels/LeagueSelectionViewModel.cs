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
        private readonly MainViewModel _main; // ViewModel principal para navegación

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

        public LeagueSelectionViewModel(MainViewModel main, LeagueService leagueService, GroupService groupService) {
            _main = main;
            _leagueService = leagueService;
            _groupService = groupService;

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
            var leagues = await _leagueService.GetLeaguesAsync();
            Leagues.Clear();
            foreach (var league in leagues)
                Leagues.Add(league);
        }

        // Abre la vista de equipos de la liga seleccionada
        private void OpenLeague() {
            if (SelectedLeague == null) return;

            _main.SelectLeague(SelectedLeague);

            var vm = new TeamListViewModel(_main.AppState, _main.TeamService, SelectedLeague.Id);
            _main.CurrentView = vm;
            _ = vm.InitializeAsync();
        }

        // Edita el nombre de la liga seleccionada
        private async Task EditLeagueAsync() {
            if (SelectedLeague == null) return;

            var window = new CreateLeagueWindow(); // Reusa ventana de crear liga
            var vm = new CreateOrEditLeagueViewModel(SelectedLeague); // ViewModel unificado para crear/editar
            window.DataContext = vm;

            // Permite que el ViewModel cierre la ventana con DialogResult
            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Editar liga";

            var result = window.ShowDialog(); // Muestra la ventana como modal

            if (result == true) {

                var success = await _leagueService.EditLeagueAsync(
                    new LeagueDto { 
                        Id = SelectedLeague.Id, 
                        Name = vm.LeagueName, 
                        RegistrationFee = vm.RegistrationFee 
                    }
                );

                if (success) {
                    var index = Leagues.IndexOf(SelectedLeague);

                    var updatedLeague = new LeagueDto {
                        Id = SelectedLeague.Id,
                        Name = vm.LeagueName,
                        RegistrationFee = vm.RegistrationFee
                    };

                    Leagues[index] = updatedLeague;
                    SelectedLeague = updatedLeague;
                }
            }
        }

        // Crea una nueva liga
        private async Task CreateLeagueAsync() {
            var window = new CreateLeagueWindow();
            var vm = new CreateOrEditLeagueViewModel(null); // Null indica creación
            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;
            window.Title = "Crear liga";

            var result = window.ShowDialog();
            if (result == true) {

                var created = await _leagueService.CreateLeagueAsync(new LeagueDto {Name = vm.LeagueName, RegistrationFee = vm.RegistrationFee });
                if (created != null) {
                    if (vm.NumberOfGroups == 0) vm.NumberOfGroups = 1;
                    for( var i = 0; i < vm.NumberOfGroups; i++) {
                        await _groupService.CreateGroupAsync(new GroupDto { Name = $"Grupo {i + 1}", LeagueId = created.Id});
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