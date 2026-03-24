using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;

namespace Fut7Manager.Admin.ViewModels {
    public class LeagueSelectionViewModel : BaseViewModel {
        private readonly LeagueService _leagueService;
        private readonly MainViewModel _main;

        public ObservableCollection<LeagueDto> Leagues { get; } = new();

        // ESTE es el ICommand que la vista usa
        public ICommand SelectLeagueCommand { get; }

        public LeagueSelectionViewModel(MainViewModel main, LeagueService leagueService) {
            _main = main;
            _leagueService = leagueService;

            // Crear comando que llama a OnLeagueSelected
            SelectLeagueCommand = new RelayCommand<LeagueDto>(OnLeagueSelected);
        }

        private void OnLeagueSelected(LeagueDto? league) {
            if (league == null) return;

            // Actualiza AppState
            _main.SelectLeague(league);

            // Navega automáticamente a TeamsView
            var vm = new TeamsViewModel(_main.AppState, _main.TeamService);
            _main.CurrentView = vm;
            _ = vm.InitializeAsync();
        }

        public async Task InitializeAsync() {
            var leagues = await _leagueService.GetLeaguesAsync();
            Leagues.Clear();
            foreach (var l in leagues)
                Leagues.Add(l);
        }
    }
}