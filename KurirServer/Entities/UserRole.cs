using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class UserRole
    {
        [Required,Key]
        public int UserRoleID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int RoleID { get; set; }
    }
}
