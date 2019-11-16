using KurirServer.Entities;
using KurirServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
     public interface IUserRepository
    {
        Task<User> GetUserAsync(int ID);
        Task<List<User>> GetAllUserAsync();
        IEnumerable<int> GetUsersIdsWithCourierRole();
        IEnumerable<int> GetUsersIdsWithDispatcherRole();
        Task<User> GetUserByEmailAsync(string Email);
        Task<bool> MakeUserActive(int id);
        Task<bool> LogoutAsync(int id);
        Task<bool> ChangeCurrentUserRole(int Userid,int UserRoleID);
        int GetCurrentUserRole(int Userid);
        Task<IEnumerable<ActiveCourierModel>> GetActiveCouriers();
        Task<IEnumerable<CourierModel>> GetAllCouriers();
        Task<IEnumerable<User>> GetAllUsersByRoleAsync(int roleID);
        Task<int> GetCountOfUsersByRoleAsync(int roleID);
    }
}
