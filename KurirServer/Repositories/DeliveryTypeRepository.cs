using KurirServer.Entities;
using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class DeliveryTypeRepository : IDeliveryTypeRepository
    {
        private KurirDbContext _context;
        public DeliveryTypeRepository(KurirDbContext context)
        {
            _context = context;
        }
        public IEnumerable<DeliveryType> GetDeliveryTypes()
        {
            return _context.DeliveryTypes.ToList();
        }
    }
}
