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
        private LeagueService _leagueService;
        private TeamService _teamService;
        private GroupService _groupService;
        private Fut7MatchService _fut7MatchService;
        private bool _isLoading;
        public ObservableCollection<GroupStandingDto> GroupedStandings { get; set; } = new();
        public Fut7MatchDto? SelectedMatch { get; set; }

        public Fut7MatchService Fut7MatchService
        {
            get => _fut7MatchService;
            set { _fut7MatchService = value; OnPropertyChanged(); }
        }

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

        private string? _currentMatchdayName = "Jornada";
        public string? CurrentMatchdayName
        {
            get => _currentMatchdayName;
            set { _currentMatchdayName = value; OnPropertyChanged(); }
        }

        public class GroupStandingDto {
            public string GroupName { get; set; } = default!;
            public List<StandingDto> Standings { get; set; } = new();
        }

        public ObservableCollection<Fut7MatchDto> NextMatches { get; set; } = new();

        private string? _championName;
        public string? ChampionName
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
        public CentralPanelViewModel(AppState appState, LeagueService leagueService, LeagueDto league, TeamService teamService, GroupService groupService, Fut7MatchService fut7MatchService) {
            _appState = appState;
            _league = league;
            _leagueService = leagueService;
            _teamService = teamService;
            _groupService = groupService;
            _fut7MatchService = fut7MatchService;

            LeagueStatus = _league.Status;

            StartLeagueCommand = new RelayCommand(async () => await StartLeague());
        }

        public async Task InitializeAsync() {
            await LoadGroupsAndTeams();

            if (LeagueStatus == LeagueStatus.InProgress) {
                await LoadDashboardData();
            }
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
        private async Task StartLeague() {
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

            _league.Status = LeagueStatus.InProgress;

            var success = await _leagueService.EditLeagueAsync(_league);

            if (success) {
                MessageBox.Show("Liga iniciada");
                // 🔹 1. Recargar liga
                _league = await _leagueService.GetLeagueByIdAsync(_league.Id);
                LeagueStatus = _league.Status;

                await LoadDashboardData();
            } else {
                MessageBox.Show("No se pudo iniciar la liga correctamente");
            }

            return;
            
        }

        private async Task LoadDashboardData() {
            IsLoading = true;

            var dashboard = await _leagueService.GetDashboardAsync(_league.Id);

            if (dashboard == null) {
                CurrentMatchdayName = "Error al cargar";
                NextMatches.Clear();
                GroupedStandings.Clear();
                IsLoading = false;
                return;
            }

            CurrentMatchdayName = dashboard.CurrentMatchday != null
                     ? $"Jornada {dashboard.CurrentMatchday.Number}"
                     : "Sin jornada activa";

            // 🔹 MATCHES (igual que ya tenías)
            NextMatches.Clear();

            if (dashboard.CurrentMatchday?.Matches != null) {
                foreach (var match in dashboard.CurrentMatchday.Matches) {
                    NextMatches.Add(match);
                }
            }

            // 🔥 STANDINGS AGRUPADOS
            GroupedStandings.Clear();

            if (dashboard.GroupedStandings != null && dashboard.GroupedStandings.Any()) {
                // 👉 Caso correcto (cuando tu API ya los manda por grupo)
                foreach (var group in dashboard.GroupedStandings) {
                    GroupedStandings.Add(new GroupStandingDto {
                        GroupName = group.GroupName,
                        Standings = group.Standings
                    });
                }
            } else if (dashboard.Standings != null) {
                // 👉 Fallback (lo que tienes ahorita)
                GroupedStandings.Add(new GroupStandingDto {
                    GroupName = "General",
                    Standings = dashboard.Standings
                });
            }

            IsLoading = false;
        }

        public async Task RefreshDashboard() {
            await LoadDashboardData();
        }
    }
}