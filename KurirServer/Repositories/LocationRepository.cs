using KurirServer.Entities;
using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private KurirDbContext context;
        public LocationRepository(KurirDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> AddLocation(Location location)
        {
            try
            {
                context.Add(location);
                int x= await context.SaveChangesAsync();
                if (x==1)
                { return true ; }
                else return false;
            }
            catch { return false; }
           

           
        }

        public async Task<Location> GetLocationByID(int ID)
        {
            return await context.Locations.ToAsyncEnumerable().Where(l => l.LocationID == ID).FirstOrDefault();
           

        }

        public async Task<Location> GetLocationByUserID(int UserID)
        {
            return await context.Locations.ToAsyncEnumerable().Where(l => l.UserID == UserID).FirstOrDefault();
             
        }

        public Task<Location> UpdateLocation(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
