using KurirServer.Entities;
using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    
    public class PaymentTypeRepository:IPaymentTypeRepository
    {
        private KurirDbContext _context;

        public PaymentTypeRepository(KurirDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PaymentType> GetPaymentTypes()
        {
           return _context.PaymentTypes.ToList();
        }
    }
}
