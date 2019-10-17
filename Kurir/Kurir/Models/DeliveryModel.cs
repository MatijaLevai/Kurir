using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Kurir.Models
{
    public class DeliveryModel
    {
        [PrimaryKey]
        public int DeliveryID { get; set; }

        public int CourierID { get; set; }
        public int UserID { get; set; }
        public int DispatcherID { get; set; }

         
        public int StartAddressID { get; set; }
        public FullAddressModel StartAddress { get; set; }

       
        public int EndAddressID { get; set; }
        public FullAddressModel EndAddress { get; set; }
        public string Description { get; set; }
        public int WaitingInMinutes { get; set; }

        public decimal DeliveryPrice { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DeliveryTypeID { get; set; }
        public int PaymentTypeID { get; set; }
        public string DeliveryStatusImageSource { get; set; }
        public int DeliveryStatus { get; set; }
    }
}
