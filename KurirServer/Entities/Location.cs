using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class Location
    {
        [Required, Key]
        public int LocationID { get; set; }
       
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public double Altitude { get; set; }

        public DateTimeOffset DToffSet { get; set; }
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
