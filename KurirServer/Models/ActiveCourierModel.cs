using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Models
{
    public class ActiveCourierModel
    {
        public int CourierID { get; set; }
        public string CourierFullName { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double Alt { get; set; }
        public DateTimeOffset DToffSet { get;set; }
    }
}
