using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.ViewModels {
    public class LeaguesViewModel : BaseViewModel {
        private readonly LeagueService _leagueService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // Colección observable de ligas
        public ObservableCollection<LeagueDto> Leagues { get; set; } = new();

        public LeaguesViewModel() {
            _leagueService = new LeagueService();
        }

        // Carga ligas desde API
        private async Task LoadLeagues() {
            var _leagues = await _leagueService.GetLeaguesAsync();
            Leagues.Clear();
            foreach (var league in _leagues) {
                Leagues.Add(league);
            }
        }

        // Inicializa y actualiza la UI con ligas
        public async Task InitializeAsync() {
            IsLoading = true;
            await LoadLeagues();
            IsLoading = false;
        }
    }
}
