using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class CentralPanelViewModel : BaseViewModel {
        private readonly AppState _appState; // ViewModel principal para navegación
        private LeagueDto _league;
        private TeamService _teamService;
        private GroupService _groupService;
        private bool _isLoading;
        public List<TeamDto> Teams { get; } = new();
        public List<GroupDto> Groups { get; } = new();

        private LeagueStatus _leagueStatus;
        public LeagueStatus LeagueStatus
        {
            get => _leagueStatus;
            set { _leagueStatus = value; OnPropertyChanged(); }
        }

        private int _totalTeams;
        public int TotalTeams
        {
            get => _totalTeams;
            set { _totalTeams = value; OnPropertyChanged(); }
        }

        private int _totalGroups;
        public int TotalGroups
        {
            get => _totalGroups;
            set { _totalGroups = value; OnPropertyChanged(); }
        }

        private string _currentMatchdayName;
        public string CurrentMatchdayName
        {
            get => _currentMatchdayName;
            set { _currentMatchdayName = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Fut7MatchDto> NextMatches { get; set; } = new();

        private string _championName;
        public string ChampionName
        {
            get => _championName;
            set { _championName = value; OnPropertyChanged(); }
        }

        public ICommand StartLeagueCommand { get; }

        
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // 🔹 Constructor
        public CentralPanelViewModel(AppState appState, LeagueDto league, TeamService teamService, GroupService groupService) {
            _appState = appState;
            _league = league;
            _teamService = teamService;
            _groupService = groupService;

            LeagueStatus = _league.Status;

            StartLeagueCommand = new RelayCommand(StartLeague);
        }

        public async Task InitializeAsync() {
            await LoadGroupsAndTeams();
        }

        private async Task LoadGroupsAndTeams() {
            //if(SelectedTeam == null) return;

            IsLoading = true;

            var teams = await _teamService.GetTeamsAsync(_league.Id);

            var groups = await _groupService.GetGroupsAsync(_league.Id);

            Teams.Clear();
            foreach (var team in teams) {
                Teams.Add(team);
            }

            Groups.Clear();
            foreach (var group in groups) {
                Groups.Add(group);
            }

            TotalTeams = teams.Count;
            TotalGroups = groups.Count;

            IsLoading = false;
        }

        //Acción principal
        private void StartLeague() {
            // Validación básica
            if (TotalTeams < 2) {
                MessageBox.Show("No hay suficientes equipos para iniciar la liga");
                return;
            }

            var window = new GroupAssignmentWindow();

            var vm = new GroupAssignmentViewModel(Teams, Groups, _league.Id);

            window.DataContext = vm;

            vm.CloseAction = result =>
            {
                window.DialogResult = result;
                window.Close();
            };

            window.ShowDialog();

            // Opcional
            MessageBox.Show("Liga iniciada 🚀");
        }
    }
}