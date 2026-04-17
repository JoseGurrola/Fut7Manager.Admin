using Fut7Manager.Admin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fut7Manager.Admin.Helpers {
    public class GroupWithTeams {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ObservableCollection<TeamDto> Teams { get; set; } = new();
    }
}
