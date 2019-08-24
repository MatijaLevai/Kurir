using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class DeliveryModel
    {
        public int DeliveryID { get; set; }

        public int CourierID { get; set; }
        public int UserID { get; set; }
        public int DispatcherID { get; set; }

        public string NameStart { get; set; }
        public string NameEnd { get; set; }

        public string PhoneOfStart { get; set; }
        public string PhoneOfEnd { get; set; }

        public string StartAddress { get; set; }

        public string EndAddress { get; set; }

        public string Description { get; set; }

        public int StartLocationID { get; set; }
        public int EndLocationID { get; set; }


        public int ZoneStart { get; set; }//1,2,3

        public int ZoneEnd { get; set; }//1,2,3

        public int WaitingInMinutes { get; set; }

        public decimal DeliveryPrice { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DeliveryTypeID { get; set; }
        public int PaymentTypeID { get; set; }

    }
}
