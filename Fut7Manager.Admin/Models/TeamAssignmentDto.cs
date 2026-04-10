using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models {
    public class TeamAssignmentDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
    }
}
