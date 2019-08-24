using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly KurirDbContext context;
        public UserRepository(KurirDbContext context)
        {
            this.context = context;
        }
        public async Task<User> GetUserAsync(int ID)
        {
            IQueryable<User> query = context.Users;

            query = query.Where(u => u.UserID == ID);
            return await query.FirstOrDefaultAsync();
         
        }

        public async Task<User> GetUserByEmailAsync(string Email)
        {
            IQueryable<User> query = context.Users;

            query = query.Where(u => u.Mail == Email);
            return await query.FirstOrDefaultAsync();
        }

      
        

        public async Task<bool> MakeUserActive(int id)
        {
            try
            {
                IQueryable<User> query = context.Users;
                query = query.Where(u => u.UserID == id);
                var usr = query.First();
                usr.IsActive = true;
                context.Update(usr);
                await context.SaveChangesAsync();

                return true;
                
            }
            catch 
            { return false; }

        }
        public async Task<bool> LogoutAsync(int id)
        {
            try
            {
                IQueryable<User> query = context.Users;
                query = query.Where(u => u.UserID == id);
                var usr = query.First();
                usr.IsActive = false;
                context.Update(usr);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            { return false; }

        }

        public async Task<bool> ChangeCurrentUserRole(int Userid, int UserRoleID)
        {
            try {
                IQueryable<User> query = context.Users;
                query = query.Where(u => u.UserID == Userid);
                var usr = query.First();
                usr.ActiveUserRoleID = UserRoleID;
                context.Update(usr);
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
