using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Fut7Manager.Admin.ViewModels {
    public class PlayersViewModel : BaseViewModel {
        private readonly PlayerService _playerService;

        public ObservableCollection<PlayerDto> Players { get; set; }

        public PlayersViewModel() {
            _playerService = new PlayerService();
            Players = new ObservableCollection<PlayerDto>();

            _ = LoadPlayers();
        }

        private async Task LoadPlayers() {
            var _players = await _playerService.GetPlayersAsync();

            Players.Clear();

            foreach (var player in _players) {
                Players.Add(player);
            }
        }
    }
}
