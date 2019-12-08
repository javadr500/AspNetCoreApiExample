using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreApiExample.Models
{
  

    public class PositionModel
    {
        [Required]
        public double? FromLat { get; set; }

        [Required]
        public double? FromLng { get; set; }

        [Required]
        public double? DistLat { get; set; }

        [Required]
        public double? DistLng { get; set; }
    }


    public class GeoHistoryModel : PositionModel
    {
        public double Distance { get; set; }
        public DateTime Date { get; set; }
    }
}
