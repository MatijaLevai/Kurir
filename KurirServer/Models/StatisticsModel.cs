using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Models
{
    public class StatisticsModel
    {
        public int UserID { get; set; }
        public string ImePrezime { get; set; }
        public int BrojDostava { get; set; }
        public double Promet { get; set; }
        public double PrihodOdPrometa { get; set; }
        public int Procenat { get; set; }
        public decimal PrometCash { get; set; }
        public decimal PrometCupon { get; set; }
        public decimal PrometFaktura { get; set; }
    }
}
