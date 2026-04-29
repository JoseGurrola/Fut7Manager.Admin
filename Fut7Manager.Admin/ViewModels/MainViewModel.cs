using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels {
    public class MainViewModel : BaseViewModel {
        private readonly AppState _appState;
        private readonly PlayerService _playerService = new PlayerService();
        private readonly TeamService _teamService = new TeamService();
        //private readonly MatchService _matchService = new MatchService();
        private readonly LeagueService _leagueService = new LeagueService();
        private readonly GroupService _groupService = new GroupService();
        private readonly Fut7MatchService _fut7MatchService = new Fut7MatchService();
        // Para que otras ViewModels puedan acceder
        public AppState AppState => _appState;
        public TeamService TeamService => _teamService;
        public PlayerService PlayerService => _playerService;
        public LeagueService LeagueService => _leagueService;
        public GroupService GroupService => _groupService;
        public Fut7MatchService Fut7MatchService => _fut7MatchService;

        private BaseViewModel? _currentView;
        public BaseViewModel? CurrentView
        {
            get => _currentView;
            set {
                _currentView = value;
                OnPropertyChanged(); // <- muy importante
            }
        }
        public LeagueDto? SelectedLeague => _appState.SelectedLeague;
        public bool IsAuthenticated { get; private set; }
        public bool CanNavigate => IsAuthenticated && _appState.SelectedLeague != null;

        public ICommand ChangeLeagueCommand { get; }
        public ICommand ShowPlayersCommand { get; }
        public ICommand ShowTeamsCommand { get; }

        public ICommand ShowCentralPanelCommand { get; }
        public ICommand ShowMatchesCommand { get; }

        public ICommand ShowStandingsCommand { get; }

        public MainViewModel(AppState appState) {
            _appState = appState;
            _appState.LeagueChanged += OnLeagueChanged;

            //ShowTeamsCommand = new RelayCommand(async () => {
            //    if (!CanNavigate) return;
            //    if (SelectedLeague == null) return;
            //    var vm = new CentralPanelViewModel(_appState, SelectedLeague.Id);
            //    CurrentView = vm;
            //    await vm.InitializeAsync();
            //});

            ShowCentralPanelCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                if (SelectedLeague == null) return;
                var vm = new CentralPanelViewModel(_appState, _leagueService, SelectedLeague, _teamService, _groupService, _fut7MatchService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowStandingsCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                if (SelectedLeague == null) return;
                var vm = new StandingsViewModel(_appState, _leagueService, SelectedLeague, _teamService, _groupService, _fut7MatchService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ChangeLeagueCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                _appState.ClearLeague();
                var vm = new LeagueSelectionViewModel(this, _leagueService, _teamService, _groupService, _fut7MatchService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowPlayersCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                var vm = new PlayersViewModel(_appState, _playerService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowTeamsCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                if(SelectedLeague == null) return;
                var vm = new TeamListViewModel(_appState, _teamService, SelectedLeague);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowMatchesCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                var vm = new MatchesViewModel(_appState, _fut7MatchService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            // Inicialmente mostrar LoginView
            CurrentView = new LoginViewModel(LoginSucceededCallback);
        }

        private void OnLeagueChanged() {
            OnPropertyChanged(nameof(SelectedLeague));
            OnPropertyChanged(nameof(CanNavigate));
        }

        public void SelectLeague(LeagueDto? league) {
            if (league == null) return;
            _appState.SetLeague(league);
            OnPropertyChanged(nameof(SelectedLeague));
            OnPropertyChanged(nameof(CanNavigate));
        }

        private async void LoginSucceededCallback() {
            IsAuthenticated = true;
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(CanNavigate));

            // Cambiamos la vista al selector de ligas
            var vm = new LeagueSelectionViewModel(this, _leagueService, _teamService, _groupService, _fut7MatchService);
            CurrentView = vm; // <- OnPropertyChanged notificará al ContentControl
            await vm.InitializeAsync();
        }
    }
}