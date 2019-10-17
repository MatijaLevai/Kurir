using KurirServer.Entities;
using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private KurirDbContext context;
        public AddressRepository(KurirDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> AddAddress(FullAddress address)
        {
            try
            {
                await context.AddAsync<FullAddress>(address);
                if (await context.SaveChangesAsync() > 0)
                    return true;
                else return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.InnerException);
                return false;
            }
        }

        public FullAddress GetAddressByID(int ID)
        {
            try
            {
               return context.Addresses.Where(a => a.FullAddressID == ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.InnerException);
                return null;
            }
        }

        public IEnumerable<FullAddress> GetAddressesByUserID(int UserID)
        {
           return context.Addresses.Where(a => a.UserID == UserID).AsEnumerable();
        }

        public async Task<bool> UpdateAddress(FullAddress address)
        {
            try { 
                    context.Addresses.Update(address);
                if (await context.SaveChangesAsync() > 0)
                    return true;
                else return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.InnerException);
                return false;
            }
        }
    }
}
