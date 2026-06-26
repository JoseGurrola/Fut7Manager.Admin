using Fut7Manager.Admin.Helpers;
using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Fut7Manager.Admin.ViewModels
{
    public class StandingsViewModel : BaseViewModel {
        private LeagueDto _league;
        private readonly LeagueService _leagueService;

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }
        private bool _isLoading;

        public enum StandingsTab {
            Groups,
            General,
            TopScorers,
            YellowCards,
            RedCards
        }

        private StandingsTab _selectedTab;
        public StandingsTab SelectedTab
        {
            get => _selectedTab;
            set { _selectedTab = value; OnPropertyChanged(); }
        }



        public ObservableCollection<GroupStandingDto> GroupedStandings { get; } = new();
        public ObservableCollection<StandingDto> GeneralStandings { get; } = new();
        public ObservableCollection<PlayerStatStandingDto> TopScorers { get; } = new();
        public ObservableCollection<PlayerStatStandingDto> YellowCards { get; } = new();
        public ObservableCollection<PlayerStatStandingDto> RedCards { get; } = new();

        public ICommand ChangeTabCommand { get; }

        public bool ShowGeneralTable => GroupedStandings.Count > 1;
        public StandingsViewModel(AppState appState, LeagueService leagueService, LeagueDto league, TeamService teamService, GroupService groupService, Fut7MatchService fut7MatchService) {
            _league = league;
            _leagueService = new LeagueService();

            ChangeTabCommand = new RelayCommand<string>(param =>
            {
                if (Enum.TryParse(param, out StandingsTab tab))
                    SelectedTab = tab;
            });


            SelectedTab = StandingsTab.Groups;
        }

        public async Task InitializeAsync() {
            

            await LoadStandings();
        }

        private async Task LoadStandings() {
            IsLoading = true;

            var result = await _leagueService.GetStandingsAsync(_league.Id);

            GroupedStandings.Clear();
            GeneralStandings.Clear();
            TopScorers.Clear();
            YellowCards.Clear();
            RedCards.Clear();

            if (result != null) {
                foreach (var group in result.GroupedStandings)
                    GroupedStandings.Add(group);

                foreach (var team in result.Standings)
                    GeneralStandings.Add(team);

                foreach (var player in result.PlayerStandings.TopScorers)
                    TopScorers.Add(player);

                foreach (var player in result.PlayerStandings.YellowCards)
                    YellowCards.Add(player);

                foreach (var player in result.PlayerStandings.RedCards)
                    RedCards.Add(player);
            }

            OnPropertyChanged(nameof(ShowGeneralTable));

            IsLoading = false;
        }

    }
}
