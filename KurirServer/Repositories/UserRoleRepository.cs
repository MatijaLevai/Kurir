using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class UserRoleRepository:IUserRoleRepository
    {
        private readonly KurirDbContext context;

        public UserRoleRepository(KurirDbContext context)
        {
            this.context = context;
        }

        public int AddUserRole(int UserID,int RoleID=3)
        {
            try
            {
                UserRole ur = new UserRole
                {
                    RoleID = RoleID,
                    UserID = UserID
                };
                Debug.WriteLine("_______________________Objekar userRole kreiran.______________________-");
                context.Add(ur);
               
                Debug.WriteLine("_______________________Objekar userRole Dodat u kontekst.______________________-");
                context.SaveChanges();
                Debug.WriteLine("_______________________Objekar userRole dodat u bazu.______________________-");

                return ur.UserRoleID;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message);
                return 0;
            }
           
        }

        public  IEnumerable<UserRole> GetUserRoles(int UserID)
        {
            IQueryable<UserRole> query =  context.UserRoles;
            return query.Where(ur => ur.UserID == UserID);
            

        }
        public IEnumerable<UserRole> GetAllUserRoles()
        {
           return context.UserRoles;
        }
        public IEnumerable<Role> GetAllRoles()
        {
            return context.Roles;
        }

        public UserRole GetUserRoleByID(int id)
        {
          return  context.UserRoles.Where(ur=>ur.UserRoleID==id).FirstOrDefault();
        }
    }
}
