using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models
{
    public class GroupStandingDto {
        public string GroupName { get; set; } = string.Empty;
        public List<StandingDto> Standings { get; set; } = new();
    }
}
