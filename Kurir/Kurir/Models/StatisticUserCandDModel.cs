using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class StatisticUserCandDModel
    {
        public int UserID { get; set; }
        public string ImePrezime { get; set; }
        public int BrojDostava { get; set; }
        public double Promet { get; set; }
        public double PrihodOdPrometa { get; set; }
        public double PrometCash { get; set; }
        public double PrometCupon { get; set; }
        public double PrometFaktura { get; set; }
        //public IEnumerable<DeliveryModel> UserCDDeliveryList{get;set;}
        private int Procenat { get; set; }
    }
}
