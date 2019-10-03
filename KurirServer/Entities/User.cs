using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KurirServer.Entities
{
    public class User
    {
        [Required, Key]
        public int UserID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Pass { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        public int ActiveUserRoleID{get;set;}
        public int Procenat { get; set; }
        public virtual ICollection<Delivery> UsersDeliveries { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public override string ToString()
        {
            return "FirstName"+ FirstName+"__"+ "LastName"+ LastName;
        }
    }
}
