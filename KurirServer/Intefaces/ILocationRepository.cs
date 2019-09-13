using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
    public interface ILocationRepository
    {
        Task<Location> GetLocationByID(int ID);
        Task<Location> GetLocationByUserID(int UserID);
        Task<bool> AddLocation(Location location);
        Task<Location> UpdateLocation(Location location);


    }
}
