using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class GroupStandingDto {
        public string GroupName { get; set; } = default!;
        public List<StandingDto> Standings { get; set; } = new();
    }
}
