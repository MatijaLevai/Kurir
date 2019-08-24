using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
    public interface IDeliveryTypeRepository
    {
        IEnumerable<DeliveryType> GetDeliveryTypes();
    }
}
