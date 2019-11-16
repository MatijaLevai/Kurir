using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class FullAddressModel
    {
        public int FullAddressID { get; set; }
        public int? UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? LocationID { get; set; }
        public int Zone { get; set; }
        public Dictionary<string, bool> CompareTo(FullAddressModel compareTo)
        {
            Dictionary<string, bool> returnStringMessage = new Dictionary<string, bool>();
            if (FullAddressID == compareTo.FullAddressID)
            {
                returnStringMessage.Add("FullAddressID", true);
            }
            else
            {
                returnStringMessage.Add("FullAddressID", false);
            }
            if (UserID == compareTo.UserID)
            {
                returnStringMessage.Add("UserID", true);
            }
            else
            {
                returnStringMessage.Add("UserID", false);
            }
            if (Name == compareTo.Name)
            {
                returnStringMessage.Add("Name", true);
            }
            else
            {
                returnStringMessage.Add("Name", false);
            }
            if (Phone == compareTo.Phone)
            {
                returnStringMessage.Add("Phone", true);
            }
            else
            {
                returnStringMessage.Add("Phone", false);
            }
            if (Address == compareTo.Address)
            {
                returnStringMessage.Add("Address", true);
            }
            else
            {
                returnStringMessage.Add("Address", false);
            }
            if (LocationID == compareTo.LocationID)
            {
                returnStringMessage.Add("LocationID", true);
            }
            else
            {
                returnStringMessage.Add("LocationID",false);
            }
            if (Zone == compareTo.Zone)
            {
                returnStringMessage.Add("Zone", true);
            }
            else
            {
                returnStringMessage.Add("Zone", false);
            }
            return returnStringMessage;
        }
        }
}
