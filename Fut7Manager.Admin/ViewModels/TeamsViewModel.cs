using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.ViewModels {
    public class TeamsViewModel : BaseViewModel {
        private readonly TeamService _TeamService;

        public ObservableCollection<TeamDto> Teams { get; set; }

        public TeamsViewModel() {
            _TeamService = new TeamService();
            Teams = new ObservableCollection<TeamDto>();

            _ = LoadTeams();
        }

        private async Task LoadTeams() {
            var _teams = await _TeamService.GetTeamsAsync();

            Teams.Clear();

            foreach (var team in _teams) {
                Teams.Add(team);
            }
        }
    }
}
