using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class LeagueSelectionViewModel : BaseViewModel {
        private readonly LeagueService _leagueService;
        private readonly MainViewModel _main;

        public ObservableCollection<LeagueDto> Leagues { get; } = new();

        public ICommand SelectLeagueCommand { get; }

        public LeagueSelectionViewModel(MainViewModel main, LeagueService leagueService) {
            _leagueService = leagueService;
            _main = main;

            SelectLeagueCommand = new RelayCommand<LeagueDto>(OnLeagueSelected);
        }

        private void OnLeagueSelected(LeagueDto? league) {
            if (league == null)
                return;

            // Actualiza AppState a través del MainViewModel
            _main.SelectLeagueCommand.Execute(league);
            // Navega a players
            _main.ShowTeamsCommand.Execute(null);
        }

        public async Task InitializeAsync() {
            var leagues = await _leagueService.GetLeaguesAsync();

            Leagues.Clear();

            foreach (var l in leagues)
                Leagues.Add(l);
        }
    }
}