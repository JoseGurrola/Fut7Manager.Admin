using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using Fut7Manager.Admin.ViewModels.SecondaryViewModels;
using Fut7Manager.Admin.Views.SecondaryWindows;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Fut7Manager.Admin.ViewModels {
    public class PlayersViewModel : BaseViewModel {
        private readonly AppState _appState;
        private readonly PlayerService _playerService;

        public ObservableCollection<PlayerDto> Players { get; } = new();

        private PlayerDto? _selectedPlayer;
        public PlayerDto? SelectedPlayer
        {
            get => _selectedPlayer;
            set {
                _selectedPlayer = value;
                OnPropertyChanged();

                (EditPlayerCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeletePlayerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand CreatePlayerCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand DeletePlayerCommand { get; }

        public PlayersViewModel(AppState appState, PlayerService playerService) {
            _appState = appState;
            _playerService = playerService;

            CreatePlayerCommand = new RelayCommand(async () => await CreatePlayerAsync());
            EditPlayerCommand = new RelayCommand(EditPlayer,() => SelectedPlayer != null);
            DeletePlayerCommand = new RelayCommand(DeletePlayer,() => SelectedPlayer != null);

            _appState.LeagueChanged += OnLeagueChanged;
        }

        private async void OnLeagueChanged() {
            if (_appState.SelectedLeague != null)
                await LoadPlayers();
            else
                Players.Clear();
        }

        public async Task InitializeAsync() {
            if (_appState.SelectedLeague != null)
                await LoadPlayers();
        }

        private async Task LoadPlayers() {
            if (_appState.SelectedLeague == null)
                return;

            IsLoading = true;

            var players = await _playerService.GetPlayersAsync(_appState.SelectedLeague.Id);

            Players.Clear();

            foreach (var p in players)
                Players.Add(p);

            IsLoading = false;
        }

        private async Task CreatePlayerAsync() {
            if (_appState.SelectedLeague == null) return;

            var window = new CreatePlayerWindow();
            var vm = new CreateOrEditPlayerViewModel(_appState.SelectedLeague);

            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;

            var result = window.ShowDialog();

            if (result == true) {
                var created = await _playerService.CreatePlayerAsync(new PlayerDto {
                    Name = vm.Name,
                    JerseyNumber = vm.JerseyNumber,
                    Phone = vm.Phone,
                    Position = vm.Position,
                    Active = vm.Active,
                    TeamId = vm.SelectedTeamId
                });

                if (created != null)
                    Players.Add(created);
            }
        }

        private async void EditPlayer() {

            if (SelectedPlayer == null)
                return;

            if (_appState.SelectedLeague == null)
                return;

            var window = new CreatePlayerWindow();

            var vm = new CreateOrEditPlayerViewModel(
                _appState.SelectedLeague,
                SelectedPlayer);

            window.DataContext = vm;

            vm.CloseAction = result => window.DialogResult = result;

            var result = window.ShowDialog();

            if (result == true) {

                var updated = await _playerService.EditPlayerAsync(
                    SelectedPlayer.Id,
                    new PlayerDto {
                        Id = SelectedPlayer.Id,
                        Name = vm.Name,
                        JerseyNumber = vm.JerseyNumber,
                        Phone = vm.Phone,
                        Position = vm.Position,
                        Active = vm.Active,
                        TeamId = vm.SelectedTeamId,
                        DateOfBirth = vm.DateOfBirth ?? DateTime.MinValue
                    });

                if (updated != null)
                    await LoadPlayers();
            }
        }

        private async void DeletePlayer() {
            if (SelectedPlayer == null)
                return;

            var result = MessageBox.Show(
                $"¿Eliminar a {SelectedPlayer.Name}?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var success = await _playerService.DeletePlayerAsync(SelectedPlayer.Id);

            if (success)
                await LoadPlayers();
        }
    }
}