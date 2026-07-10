using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class PlayoffTeamDto : BaseViewModel {
        public int TeamId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public int Position { get; set; }

        public int Points { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public bool IsQualified { get; set; }
    }
}
