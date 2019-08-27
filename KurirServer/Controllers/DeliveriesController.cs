using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        IDeliveryRepository deliveryRepository;
        IGeneralRepository generalRepository;
        IMapper mapper;
        public DeliveriesController(IGeneralRepository generalRepository, IMapper mapper,IDeliveryRepository deliveryRepository)
        {
            this.deliveryRepository = deliveryRepository;
            this.generalRepository = generalRepository;
            this.mapper = mapper;
        }
        [HttpPost]
        [Route("NewDelivery")]
        public async Task<ActionResult<Delivery>> NewDelivery(Delivery newDelivery)
        {
            try
            {
                generalRepository.Add<Delivery>(newDelivery);
                await generalRepository.SaveChangesAsync();
                return Ok(newDelivery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                
            }
            



        }
        [HttpPut]
        [Route("EditDelivery")]
        public async Task<ActionResult<Delivery>> EditDelivery(Delivery newDelivery)
        {
            try
            {
               var delivery =  await deliveryRepository.EditDelivery(newDelivery);
                if (delivery!=null)
                    {
                        return Ok(delivery);
                    }
                    else
                    {
                        return BadRequest("Could not edit");
                    }
                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }




        }



        [Route("GetDeliveriesForUser/{Userid}")]
        [HttpGet]
        public  ActionResult<IEnumerable<Delivery>> GetDeliveriesForUser(int Userid)
        {
            try
            {
                var deliveries =  deliveryRepository.GetAllDeliveriesAsUser(Userid);
                if (deliveries!=null)
                    return Ok(deliveries);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }

}