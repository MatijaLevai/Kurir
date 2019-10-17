using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class FullAddressModel
    {
        public int FullAddressID { get; set; }
        public int? UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? LocationID { get; set; }
        public int Zone { get; set; }
    }
}
