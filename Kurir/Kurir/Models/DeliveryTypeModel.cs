using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Kurir.Models
{
    public class DeliveryTypeModel
    {
        [PrimaryKey]
        public int DeliveryTypeID { get; set; }
        public string DeliveryTypeName { get; set; }
      
    }
}
