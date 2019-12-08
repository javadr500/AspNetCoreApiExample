using System.ComponentModel.DataAnnotations;

namespace AspNetCoreApiExample.Models
{
    public class PositionModel
    {
        [Required] public double? FromLat { get; set; }

        [Required] public double? FromLng { get; set; }

        [Required] public double? DistLat { get; set; }

        [Required] public double? DistLng { get; set; }
    }
}