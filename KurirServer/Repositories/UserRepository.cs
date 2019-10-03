using KurirServer.Entities;
using KurirServer.Intefaces;
using KurirServer.Models;
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
        public IEnumerable<int> GetUsersIdsWithCourierRole()
        {
            
            IDictionary<int,string> keyValuePairs = new Dictionary<int, string>();
            keyValuePairs.Add(4,"courier");
            IQueryable<UserRole> userRoles = context.UserRoles.Where(r=>keyValuePairs.ContainsKey( r.RoleID));
            List<int> usersIDs = new List<int>();
            foreach (var item in userRoles)
            {
               var user =  context.Users.Where(u => u.UserID == item.UserID).First();
                if (!usersIDs.Contains(user.UserID))
                    usersIDs.Add(user.UserID);
            }

            //if(usersIDs.Count()>0) 
            return usersIDs;


        }
        public IEnumerable<int> GetUsersIdsWithDispatcherRole()
        {

            IDictionary<int, string> keyValuePairs = new Dictionary<int, string>();
            keyValuePairs.Add(5, "dispatcher");
            IQueryable<UserRole> userRoles = context.UserRoles.Where(r => keyValuePairs.ContainsKey(r.RoleID));
            List<int> usersIDs = new List<int>();
            foreach (var item in userRoles)
            {
                var user = context.Users.Where(u => u.UserID == item.UserID).First();
                if (!usersIDs.Contains(user.UserID))
                    usersIDs.Add(user.UserID);
            }

            //if(usersIDs.Count()>0) 
            return usersIDs;


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
                usr.ActiveUserRoleID = 0;
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
        public async Task<IEnumerable<ActiveCourierModel>> GetActiveCouriers()
        {
            try
            {
                List<ActiveCourierModel> activeCourierList = new List<ActiveCourierModel>();
                var listOfActiveUsers = context.Users.Where(ur => ur.IsActive == true).ToList();
                var listOfUserRoles = context.UserRoles.Where(ur => ur.RoleID == 4).ToList();
                foreach (var item in listOfUserRoles)
                {
                    var usr = listOfActiveUsers.Where(u =>u.ActiveUserRoleID == item.UserRoleID).FirstOrDefault();
                    if (usr != null)
                    {
                        ActiveCourierModel c = new ActiveCourierModel()
                        {
                            CourierFullName = usr.FirstName + " " + usr.LastName,
                            CourierID = usr.UserID
                        };
                        activeCourierList.Add(c);
                    }
                }
                if (activeCourierList.Count() > 0)
                {
                    List<Location> LocationList = context.Locations.ToList();
                    if (LocationList != null)
                        foreach (var item in activeCourierList)
                        {
                            Location l = LocationList.Where(loc => (loc.UserID == item.CourierID)&&(CompareDateTimeOffSEt(loc.DToffSet,DateTimeOffset.Now))).FirstOrDefault();
                            if (l != null)
                            {
                                item.Lat = l.Latitude;
                                item.Long = l.Longitude;
                                item.Alt = l.Altitude;
                                item.DToffSet = l.DToffSet;
                            }

                        }
                    await context.SaveChangesAsync();
                    return activeCourierList;
                }
               return null;
            }
            catch { return null; }
        }
        private bool CompareDateTimeOffSEt(DateTimeOffset dt1, DateTimeOffset dt2)
        {
            if ((dt1.Date == dt2.Date)&&(dt1.Hour==dt2.Hour)&&(dt2.Minute-5<dt1.Minute)&&( dt1.Minute<dt2.Minute))
                return true;
            else
            return false;
        }
    }
}
