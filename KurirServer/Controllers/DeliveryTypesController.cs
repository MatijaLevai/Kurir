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
    public class DeliveryTypesController : ControllerBase
    {
        private IGeneralRepository generalRepository;
        private IDeliveryTypeRepository deliveryTypeRepository;
        public DeliveryTypesController(IGeneralRepository generalRepository, IDeliveryTypeRepository deliveryTypeRepository)
        {
            this.deliveryTypeRepository = deliveryTypeRepository;
            this.generalRepository = generalRepository;
        }
        [HttpGet]
        [Route("GetDeliveryTypes")]
        public ActionResult<IEnumerable<DeliveryType>> GetDeliveryTypes()
        {
            var returnTypes = deliveryTypeRepository.GetDeliveryTypes();
            if (returnTypes != null)
                return Ok(returnTypes);
            else return NotFound();
        }
    }
    
}