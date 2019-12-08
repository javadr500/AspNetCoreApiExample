using System;
using System.ComponentModel.DataAnnotations;
using AspNetCoreApiExample.Models;

namespace AspNetCoreApiExample.Domain
{
    public class GeoHistory 
    {
        [Key]
        public int GeoHistoryId { get; set; }

        [Required]
        public double FromLat { get; set; }

        [Required]
        public double FromLng { get; set; }

        [Required]
        public double DistLat { get; set; }

        [Required]
        public double DistLng { get; set; }


        public double Distance { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public User User{ get; set; }
    }
}