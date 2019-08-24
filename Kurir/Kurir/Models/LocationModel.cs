using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class LocationModel
    {
        [PrimaryKey,AutoIncrement]
        public int LocationID { get; set; }
        public int userID { get; set; }
      
        public DateTime dateTime { get; set; }
        public double Latitude { get; set; }
      
        public double Longitude { get; set; }
    }
}
