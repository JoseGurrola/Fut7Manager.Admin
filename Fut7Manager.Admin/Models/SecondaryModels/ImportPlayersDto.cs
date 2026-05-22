using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows;

namespace Fut7Manager.Admin.Models.SecondaryModels
{
    public class ImportPlayersDto {

        [Required]

        public required List<PlayerDto> Players { get; set; }
    }
}
