using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ODataController
    {
        IDeliveryRepository deliveryRepository;
        IGeneralRepository generalRepository;
        IMapper mapper;
        public DeliveriesController
        (IGeneralRepository generalRepository, IMapper mapper,IDeliveryRepository deliveryRepository)
        {
            this.deliveryRepository = deliveryRepository;
            this.generalRepository = generalRepository;
            this.mapper = mapper;
        }

        [EnableQuery][HttpGet][Route("ODataGet")]
        public IQueryable<Delivery> ODataGet()
        {
            return deliveryRepository.ODataGet();
        }

        [HttpPost][Route("NewDelivery")]
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

        [HttpPut][Route("EditDelivery")]
        public async Task<ActionResult<Delivery>> EditDelivery(Delivery newDelivery)
        {
            try
            {
                //var newDelivery = JsonConvert.DeserializeObject<Delivery>(newDeliveryJson);
                // var delivery =  await deliveryRepository.EditDelivery(newDelivery);
                var delivery = await generalRepository.Update<Delivery>(newDelivery);

                if (delivery)
                    {
                        return Ok(newDelivery);
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



        [EnableQuery]
        [Route("GetDeliveries")]
        [HttpGet]
        public IEnumerable<Delivery> GetDeliveries(int Userid)
        {
            var d = (IEnumerable<Delivery>) deliveryRepository.ODataGet();
            if (d != null)
                return d;
            else return null;
        }
        [EnableQuery]
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
        [Route("GetDeliveriesForDispatcher/{Userid}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetDeliveriesForDispatcher(int Userid=0)
        {
            try
            {
                var deliveries = deliveryRepository.GetAllDeliveriesAsDispatcher(Userid);
                if (deliveries != null)
                    return Ok(deliveries);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("GetUncofirmedDeliveriesForDispatcher")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetUncofirmedDeliveriesForDispatcher()
        {
            try
            {
                var deliveries = deliveryRepository.GetUncofirmedForDispatcher();
                if (deliveries != null)
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