using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class UserRoleModel
    {
        public int UserRoleID { get; set; }
      
        public int UserID { get; set; }
       
        public int RoleID { get; set; }
        public string Name { get; set; }
    }
}
