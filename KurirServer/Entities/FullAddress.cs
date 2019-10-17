using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class FullAddress
    {
        [Required, Key]
        public int FullAddressID { get; set; }
        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? LocationID { get; set; }
        [ForeignKey("LocationID")]
        public Location Location { get; set; }

        public int Zone { get; set; }

    }
}
