using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class ActiveCourierModel
    {
        public int CourierID { get; set; }
        public string CourierFullName { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double Alt { get;set; }
        public string StatusImageSource { get; set; }
        public DateTimeOffset DToffSet { get; set; }


    }
}
