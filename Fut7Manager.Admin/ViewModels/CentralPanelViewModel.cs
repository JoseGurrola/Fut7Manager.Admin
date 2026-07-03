using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Models.SecondaryModels;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views;
using Fut7Manager.Admin.Views.SecondaryWindows;

using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace Fut7Manager.Admin.ViewModels {
    public class CentralPanelViewModel : BaseViewModel {
        private readonly AppState _appState; // ViewModel principal para navegación
        private LeagueDto _league;
        private LeagueService _leagueService;
        private TeamService _teamService;
        private GroupService _groupService;
        private Fut7MatchService _fut7MatchService;
        private LeagueStatus _leagueStatus;
        private int _totalTeams;
        private int _totalGroups;
        private int _minPlayers;
        private int _teamsQualified;
        private bool _isLoading;
        private string? _currentMatchdayName = "Jornada";
        private string? _championName;

        public class GroupStandingDto {
            public string GroupName { get; set; } = default!;
            public List<StandingDto> Standings { get; set; } = new();
        }
        public ObservableCollection<GroupStandingDto> GroupedStandings { get; set; } = new();
        public ObservableCollection<PlayerStatStandingDto> TopScorers { get; set; } = new();
        public ObservableCollection<PlayerStatStandingDto> YellowCards { get; set; } = new();
        public ObservableCollection<PlayerStatStandingDto> RedCards { get; set; } = new();
        public ObservableCollection<TeamDto> Teams { get; } = new();
        
        public ObservableCollection<Fut7MatchDto> NextMatches { get; set; } = new();
        public Fut7MatchDto? SelectedMatch { get; set; }

        private PaymentSummaryDto _summary;
        public Fut7MatchService Fut7MatchService
        {
            get => _fut7MatchService;
            set { _fut7MatchService = value; OnPropertyChanged(); }
        }

        public PlotModel DonutModel { get; private set; }

       
        public PaymentSummaryDto Summary
        {
            get => _summary;
            set {
                _summary = value;
                OnPropertyChanged(nameof(Summary));
                OnPropertyChanged(nameof(ShowDonut)); // notificar también ShowDonut
            }
        }

        public bool ShowDonut => Summary != null && Summary.TotalDue > 0;
       
        public List<GroupDto> Groups { get; } = new();

        
        public LeagueStatus LeagueStatus
        {
            get => _leagueStatus;
            set { _leagueStatus = value; OnPropertyChanged(); }
        }

        
        public int TotalTeams
        {
            get => _totalTeams;
            set { _totalTeams = value; OnPropertyChanged(); }
        }
        
        public int TotalGroups
        {
            get => _totalGroups;
            set { _totalGroups = value; OnPropertyChanged(); }
        }
        
        public int MinPlayers
        {
            get => _minPlayers;
            set { _minPlayers = value; OnPropertyChanged(); }
        }
        public int TeamsQualified
        {
            get => _teamsQualified;
            set { _teamsQualified = value; OnPropertyChanged(); }
        }
       
        public string? CurrentMatchdayName
        {
            get => _currentMatchdayName;
            set { _currentMatchdayName = value; OnPropertyChanged(); }
        }



        
        
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

        public bool HasIncompleteTeams => Teams.Any(t => t.NumPlayers < _league.MinPlayers);

        public string LeagueReadyMessage => HasIncompleteTeams
        ? $"Hay equipos con menos de {_league.MinPlayers} jugadores"
        : "Todo listo para comenzar";

        public bool CanStartPlayoff => _league.Status == LeagueStatus.InProgress;

        // 🔹 Constructor
        public CentralPanelViewModel(AppState appState, LeagueService leagueService, LeagueDto league, TeamService teamService, GroupService groupService, Fut7MatchService fut7MatchService) {
            _appState = appState;
            _league = league;
            _leagueService = leagueService;
            _teamService = teamService;
            _groupService = groupService;
            _fut7MatchService = fut7MatchService;

            LeagueStatus = _league.Status;
            MinPlayers = _league.MinPlayers ?? 0;
            TeamsQualified = _league.TotalQualifiedTeams;

            _summary = new PaymentSummaryDto {
                TotalPaid = 0,
                TotalDue = 0
            };

           
            DonutModel = new PlotModel();
        

            StartLeagueCommand = new RelayCommand(async () => await StartLeague(), () => !HasIncompleteTeams);
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
                team.IsIncomplete = team.NumPlayers < _league.MinPlayers;
                Teams.Add(team);

            }

            Groups.Clear();
            foreach (var group in groups) {
                Groups.Add(group);
            }

            TotalTeams = teams.Count;
            TotalGroups = groups.Count;

            OnPropertyChanged(nameof(HasIncompleteTeams));
            OnPropertyChanged(nameof(LeagueReadyMessage));

            (StartLeagueCommand as RelayCommand)?.RaiseCanExecuteChanged();

            IsLoading = false;


        }

        //Acción principal
        private async Task StartLeague() {
            // Validación básica
            if (TotalTeams < 2) {
                MessageService.Show("No hay suficientes equipos para iniciar la liga", "Error");
                return;
            }

            var window = new GroupAssignmentWindow();

            var vm = new GroupAssignmentViewModel(Teams, Groups, _league.Id);

            window.DataContext = vm;

            vm.CloseAction = result => {
                window.DialogResult = result;
                window.Close();
            };

            window.ShowDialog();
            if (vm.CompletedSuccessfully) {
                MessageService.Show("Liga iniciada");

                //Recargar liga
                _league = await _leagueService.GetLeagueByIdAsync(_league.Id);
                LeagueStatus = _league.Status;

                //await LoadDashboardData();

                // 🔹 Reemplazar el VM en MainViewModel
                var mainVm = Application.Current.MainWindow.DataContext as MainViewModel;
                if (mainVm != null) {
                    var newVm = new CentralPanelViewModel(
                        mainVm.AppState,
                        mainVm.LeagueService,
                        _league,
                        mainVm.TeamService,
                        mainVm.GroupService,
                        mainVm.Fut7MatchService);

                    mainVm.CurrentView = newVm;
                    await newVm.InitializeAsync();
                }
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
                // PLAYER STANDINGS
                TopScorers.Clear();
                YellowCards.Clear();
                RedCards.Clear();

                IsLoading = false;
                return;
            }

            CurrentMatchdayName = dashboard.CurrentMatchday != null
                     ? $"Jornada {dashboard.CurrentMatchday.Number}"
                     : "Sin jornada activa";

            //MATCHES
            NextMatches.Clear();

            if (dashboard.CurrentMatchday?.Matches != null) {
                foreach (var match in dashboard.CurrentMatchday.Matches) {
                    NextMatches.Add(match);
                }
            }

            //STANDINGS AGRUPADOS
            GroupedStandings.Clear();

            if (dashboard.GroupedStandings != null && dashboard.GroupedStandings.Any()) {
                //  Caso correcto (cuando tu API ya los manda por grupo)
                foreach (var group in dashboard.GroupedStandings) {
                    GroupedStandings.Add(new GroupStandingDto {
                        GroupName = group.GroupName,
                        Standings = group.Standings
                    });
                }
            } else if (dashboard.Standings != null) {
                //  Fallback (lo que tienes ahorita)
                GroupedStandings.Add(new GroupStandingDto {
                    GroupName = "General",
                    Standings = dashboard.Standings
                });
            }

            // PLAYER STANDINGS
            TopScorers.Clear();
            YellowCards.Clear();
            RedCards.Clear();

            if (dashboard.PlayerStandings != null) {
                if (dashboard.PlayerStandings.TopScorers != null) {
                    foreach (var scorer in dashboard.PlayerStandings.TopScorers)
                        TopScorers.Add(scorer);
                }

                if (dashboard.PlayerStandings.YellowCards != null) {
                    foreach (var yc in dashboard.PlayerStandings.YellowCards)
                        YellowCards.Add(yc);
                }

                if (dashboard.PlayerStandings.RedCards != null) {
                    foreach (var rc in dashboard.PlayerStandings.RedCards)
                        RedCards.Add(rc);
                }
            }

            Summary = dashboard.PaymentSummary;
           
            BuildDonut();

            IsLoading = false;
        }

        
        private void BuildDonut() {
            var model = new PlotModel();

            var series = new PieSeries {
                InnerDiameter = 0.4,          // más grueso (menor diámetro interno)
                StrokeThickness = 1,
                AngleSpan = 360,
                StartAngle = -90,
                InsideLabelFormat = "{2:0.##}%",     // porcentaje dentro del slice
                InsideLabelColor = OxyColors.Black,   // etiquetas internas en blanco
                OutsideLabelFormat = "{1}: {0:$0}"   // nombre + valor real afuera
            };

            var primary = (Color)Application.Current.Resources["SidebarHoverColor"];

            var azulHex = OxyColor.FromArgb(primary.A, primary.R, primary.G, primary.B);

            series.Slices.Add(new PieSlice("Pagado", (double)Summary.TotalPaid) { Fill = azulHex });
            series.Slices.Add(new PieSlice("Por pagar", (double)(Summary.TotalDue - Summary.TotalPaid)) { Fill = OxyColors.LightGray});

            model.Series.Add(series);
            DonutModel = model;

            OnPropertyChanged(nameof(DonutModel));
            OnPropertyChanged(nameof(Summary));
        }

    }
}