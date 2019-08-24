using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {
        private IGeneralRepository generalRepository;
        private IPaymentTypeRepository paymentTypeRepository;
        public PaymentTypesController(IGeneralRepository generalRepository, IPaymentTypeRepository paymentTypeRepository)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.generalRepository = generalRepository;
        }
        [HttpGet]
        [Route("GetPaymentTypes")]
        public ActionResult<IEnumerable<PaymentType>> GetPaymentTypes()
        {
           var returnTypes =  paymentTypeRepository.GetPaymentTypes();
            if (returnTypes != null)
                return Ok(returnTypes);
            else return NotFound();
        }
    }
}