using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models
{
    public class GroupDto {
        public int? Id { get; set; }
        public string Name { get; set; } = default!;
        public int LeagueId { get; set; }
    }
}
