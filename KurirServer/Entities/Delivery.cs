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
       
        public int DeliveryTypeID { get; set; }
        [ForeignKey("DeliveryTypeID")]
        public DeliveryType DeliveryType { get; set; }
        public int PaymentTypeID { get; set; }
        [ForeignKey("PaymentTypeID")]
        public PaymentType PaymentType { get; set; }
        public int? StartAddressID { get; set; }
        [ForeignKey("StartAddressID")]
        public FullAddress StartAddress { get; set; }
        public int? EndAddressID { get; set; }
        [ForeignKey("EndAddressID")]
        public FullAddress EndAddress { get; set; }

        public int UserID { get; set; }
        public int DispatcherID { get; set; }
        public int CourierID { get; set; }

       
        public string Description { get; set; }
        public int WaitingInMinutes { get; set; }
        
        public decimal DeliveryPrice { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime StartTime { get; set; }
       
        public DateTime EndTime { get; set; }
        
          public int DeliveryStatus { get; set; }//0==created,1==Dispatcher Edited,2==Courier acepted,3==courier Picked Up,4 delivered
    }
}
