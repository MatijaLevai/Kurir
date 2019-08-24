using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Entities
{
    public class Role
    {
        [Required, Key]
        public int RoleID { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
