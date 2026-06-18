using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels {
    public class PlayerBasicDto {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int JerseyNumber { get; set; }
    }

}
