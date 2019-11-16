using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class ExtendedDeliveryModel:DeliveryModel
    {
       
        public CourierModel Courier { get; set; }
        
        public string DeliveryStatusMessage { get; set; }
    }
}
