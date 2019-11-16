using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class CourierModel
    {
        [PrimaryKey]
        public int CourierID { get; set; }
        public string CourierFullName { get; set; }
    }

}
