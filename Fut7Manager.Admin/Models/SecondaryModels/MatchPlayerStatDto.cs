using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels {
    public class MatchPlayerStatDto : BaseViewModel {
        private string _playerName = string.Empty;
        private int _goals;
        private int _yellowCards;
        private int _redCards;
        private int? _jerseyNumber;
    
        public int? PlayerId { get; set; }

        public string PlayerName
        {
            get => _playerName;
            set => SetProperty(ref _playerName, value);
        }

        public int Goals
        {
            get => _goals;
            set => SetProperty(ref _goals, value);
        }

        public int YellowCards
        {
            get => _yellowCards;
            set => SetProperty(ref _yellowCards, value);
        }

        public int RedCards
        {
            get => _redCards;
            set => SetProperty(ref _redCards, value);
        }

        public int? JerseyNumber
        {
            get => _jerseyNumber;
            set => SetProperty(ref _jerseyNumber, value);
        }
    }
}
