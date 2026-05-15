using System;
using System.Collections.Generic;
using System.Text;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class FinalMatchdayDto {
        public int Number { get; set; }
        public List<FinalMatchDto> Matches { get; set; } = new();
    }
}
