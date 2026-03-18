using Fut7Manager.Admin.Models;
using Fut7Manager.Admin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.ViewModels {
    public class MatchesViewModel : BaseViewModel {
        private readonly MatchService _matchService;

        public ObservableCollection<Fut7MatchDto> Matches { get; set; }

        public MatchesViewModel() {
            _matchService = new MatchService();
            Matches = new ObservableCollection<Fut7MatchDto>();

           _ = LoadMatches();
        }

        private async Task LoadMatches() {
            var _matches = await _matchService.GetMatchesAsync();

            Matches.Clear();

            foreach (var match in _matches) {
                Matches.Add(match);
            }
        }
    }
}
