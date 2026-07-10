using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Fut7Manager.Admin.Models.SecondaryModels.PlayoffBracketModels
{
    public class PlayoffBracketRoundDto {
        public string Name { get; set; } = "";

        public int RoundNumber { get; set; }

        public bool IsMirrored { get; set; }

        public double MarginTop { get; set; }

        public double MatchMargin { get; set; }


        public ObservableCollection<PlayoffBracketMatchDto> Matches { get; set; }
            = new();

        public Thickness RoundMargin =>
    new Thickness(15, MarginTop, 15, 0);
    }
}
