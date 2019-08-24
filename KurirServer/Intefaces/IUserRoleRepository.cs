using KurirServer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
    public interface IUserRoleRepository
    {

        IEnumerable<UserRole> GetUserRoles(int UserID);
        int AddUserRole(int UserID, int RoleID = 3);

    }
}
