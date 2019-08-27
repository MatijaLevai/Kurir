using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace Kurir.Models
{
    public class PaymentTypeModel
    {
        [PrimaryKey]
        public int PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
    }
}
