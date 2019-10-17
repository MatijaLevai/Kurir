using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
    public interface IAddressRepository
    {
        FullAddress GetAddressByID(int ID);
        IEnumerable<FullAddress> GetAddressesByUserID(int UserID);
        Task<bool> AddAddress(FullAddress address);
        Task<bool> UpdateAddress(FullAddress address);
    }
}
