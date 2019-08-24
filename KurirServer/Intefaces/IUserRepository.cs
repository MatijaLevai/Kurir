using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
     public interface IUserRepository
    {
        Task<User> GetUserAsync(int ID);
       
        Task<User> GetUserByEmailAsync(string Email);
        Task<bool> MakeUserActive(int id);
        Task<bool> LogoutAsync(int id);
        Task<bool> ChangeCurrentUserRole(int Userid,int UserRoleID);
    }
}
