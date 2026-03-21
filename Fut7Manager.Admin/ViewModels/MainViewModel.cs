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
        private readonly MatchService _matchService = new MatchService();
        private readonly LeagueService _leagueService = new LeagueService();

        public ICommand ChangeLeagueCommand { get; }
        public ICommand SelectLeagueCommand { get; }
        public ICommand ShowPlayersCommand { get; }
        public ICommand ShowTeamsCommand { get; }
        public ICommand ShowMatchesCommand { get; }
        public ICommand ShowLeaguesCommand { get; }

        public LeagueDto? SelectedLeague => _appState.SelectedLeague;

        private BaseViewModel? _currentView;
        public BaseViewModel? CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // --------------------
        // NUEVO: Propiedad de autenticación pública
        // --------------------
        public bool IsAuthenticated { get; private set; }

        public bool CanNavigate => _appState.SelectedLeague != null && IsAuthenticated;

        public MainViewModel(AppState appState) {
            _appState = appState;
            _appState.LeagueChanged += OnLeagueChanged;

            ChangeLeagueCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                _appState.ClearLeague();
                var vm = new LeagueSelectionViewModel(this, _leagueService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            SelectLeagueCommand = new RelayCommand<LeagueDto?>(SelectLeague);

            ShowPlayersCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                var vm = new PlayersViewModel(_appState, _playerService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowTeamsCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                var vm = new TeamsViewModel(_appState, _teamService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowMatchesCommand = new RelayCommand(async () => {
                if (!CanNavigate) return;
                var vm = new MatchesViewModel(_appState, _matchService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });

            ShowLeaguesCommand = new RelayCommand(async () => {
                if (!IsAuthenticated) return;
                var vm = new LeagueSelectionViewModel(this, _leagueService);
                CurrentView = vm;
                await vm.InitializeAsync();
            });
        }

        // --------------------
        // MÉTODO PÚBLICO para abrir selección de liga tras login
        // --------------------
        public async Task OpenLeagueSelectionAfterLoginAsync() {
            IsAuthenticated = true;
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(CanNavigate));

            var leagueVm = new LeagueSelectionViewModel(this, _leagueService);
            CurrentView = leagueVm;
            await leagueVm.InitializeAsync();
        }

        // --------------------
        // Selección de liga
        // --------------------
        private void SelectLeague(LeagueDto? league) {
            if (league == null) return;
            _appState.SetLeague(league);
        }

        private void OnLeagueChanged() {
            OnPropertyChanged(nameof(SelectedLeague));
            OnPropertyChanged(nameof(CanNavigate));
        }

        // --------------------
        // Inicialización (opcional)
        // --------------------
        public async Task InitializeAsync() {
            // Ya no mostramos login aquí, eso se hace en LoginWindow
            await Task.CompletedTask;
        }
    }
}