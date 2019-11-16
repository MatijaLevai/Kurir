using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Models
{
    public class RegistrationModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Pass { get; set; }
        public string PassConfirm { get; set; }
        //public int CurentLocation { get; set; }
        public string Message { get; set; }
        public int Procenat { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int ActiveUserRoleID { get; set; }
    }
}
