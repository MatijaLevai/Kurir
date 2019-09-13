using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class Delivery
    {
        [Required,Key]
        public int DeliveryID { get; set; }
       
       
        public int UserID { get; set; }
        public int DispatcherID { get; set; }
        public int CourierID { get; set; }

        public string NameStart { get; set; }
        public string NameEnd { get; set; }

        public string PhoneOfStart { get; set; }
        public string PhoneOfEnd { get; set; }
        
        public string StartAddress { get; set; }
        
        public string EndAddress { get; set; }

        public string Description { get; set; }

        public int? StartLocationID { get; set; }
        [ForeignKey("StartLocationID")]
        public  Location Startlocation { get; set; }
        public int? EndLocationID { get; set; }
        [ForeignKey("EndLocationID")]
        public  Location Endlocation { get; set; }

        public int ZoneStart { get; set; }//1,2,3
      
        public int ZoneEnd { get; set; }//1,2,3

        public int WaitingInMinutes { get; set; }
        
        public decimal DeliveryPrice { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime StartTime { get; set; }
       
        public DateTime EndTime { get; set; }
        
        public int DeliveryTypeID { get; set; }
        [ForeignKey("DeliveryTypeID")]
        public  DeliveryType DeliveryType { get; set; }
        public int PaymentTypeID { get; set; }
        [ForeignKey("PaymentTypeID")]
        public  PaymentType PaymentType { get; set; }
        public int DeliveryStatus { get; set; }//0==created,1==Courier acepted,2==courier Picked Up,3 delivered
    }
}
