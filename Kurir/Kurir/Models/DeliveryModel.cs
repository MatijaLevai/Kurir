using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public ExtendedDeliveryModel ConvertToExtended()
        {
            ExtendedDeliveryModel extendedDeliveryModel = new ExtendedDeliveryModel
            {
                DeliveryID = this.DeliveryID,
                CourierID = CourierID,
                UserID = UserID,
                DispatcherID = DispatcherID,
                StartAddressID = StartAddressID,
                StartAddress = new FullAddressModel {
                    Address = StartAddress.Address,
                    Phone = StartAddress.Phone,
                    Name = StartAddress.Name,
                    FullAddressID = StartAddress.FullAddressID,
                    LocationID = StartAddress.LocationID,
                    Zone = StartAddress.Zone,
                    UserID = StartAddress.UserID },
                EndAddressID = EndAddressID,
                EndAddress = new FullAddressModel {
                    Address =EndAddress.Address,
                    FullAddressID =EndAddress.FullAddressID,
                    Phone = EndAddress.Phone,
                    Zone = EndAddress.Zone,
                    LocationID = EndAddress.LocationID,
                    Name = EndAddress.Name,

                    UserID = EndAddress.UserID},
                Description = Description,
                WaitingInMinutes = WaitingInMinutes,
                DeliveryPrice = DeliveryPrice,
                CreateTime = CreateTime,
                StartTime = StartTime,
                EndTime = EndTime,
                DeliveryTypeID = DeliveryTypeID,
                PaymentTypeID = PaymentTypeID,
                DeliveryStatusImageSource = DeliveryStatusImageSource,
                DeliveryStatus = DeliveryStatus
            };
            return extendedDeliveryModel;
            }

        public Dictionary<string,bool> CompareTo(DeliveryModel compareTo)
        {
            Dictionary<string, bool> returnStringMessage = new Dictionary<string, bool>();
            if (DeliveryPrice == compareTo.DeliveryPrice)
            {
                returnStringMessage.Add("DeliveryPrice", true);
            }
            else
            {
                returnStringMessage.Add("DeliveryPrice", false);
            }
            if(DeliveryID == compareTo.DeliveryID)
            {
                returnStringMessage.Add("DeliveryID", true);
            }
            else
            {
                returnStringMessage.Add("DeliveryID", false);
            }
            
            if (StartAddressID == compareTo.StartAddressID)
            {
                returnStringMessage.Add("StartAddressID", true);
            }
            else
            {
                returnStringMessage.Add("StartAddressID", false);
            }
            
            if (EndAddressID == compareTo.EndAddressID)
            {
                returnStringMessage.Add("EndAddressID", true);
            }
            else
            {
                returnStringMessage.Add("EndAddressID", false);
            }
                if (CreateTime == compareTo.CreateTime)
                {
                    returnStringMessage.Add("CreateTime", true);
                }
                else
                {
                    returnStringMessage.Add("CreateTime", false);
                }
            
            if (PaymentTypeID == compareTo.PaymentTypeID)
            {
                returnStringMessage.Add("PaymentTypeID", true);
            }
            else
            {
                returnStringMessage.Add("PaymentTypeID", false);
            }
            if (DeliveryTypeID == compareTo.DeliveryTypeID)
            {
                returnStringMessage.Add("DeliveryTypeID", true);
            }
            else
            {
                returnStringMessage.Add("DeliveryTypeID", false);
            }
            if (Description==compareTo.Description)
            {
                returnStringMessage.Add("Description", true);
            }
            else
            {
                returnStringMessage.Add("Description", false);
            }
            
            
            if (DeliveryStatus==compareTo.DeliveryStatus)
            {
                returnStringMessage.Add("DeliveryStatus", true);
            }
            else
            {
                returnStringMessage.Add("DeliveryStatus", false);
            }
            if (CourierID == compareTo.CourierID)
            {
                returnStringMessage.Add("CourierID", true);
            }
            else
            {
                returnStringMessage.Add("CourierID", false);
            }
            if (UserID == compareTo.UserID)
            {
                returnStringMessage.Add("UserID", true);
            }
            else
            {
                returnStringMessage.Add("UserID", false);
            }
            if (DispatcherID == compareTo.DispatcherID)
            {
                returnStringMessage.Add("DispatcherID", true);
            }
            else
            {
                returnStringMessage.Add("DispatcherID", false);
            }
            return returnStringMessage;
        }
    }
}
