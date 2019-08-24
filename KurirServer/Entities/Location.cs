using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class Location
    {
        [Required, Key]
        public int LocationID { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public int UserID { get; set; }
    }
}
