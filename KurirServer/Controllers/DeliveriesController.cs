using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KurirServer.Entities;
using KurirServer.Intefaces;
using KurirServer.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KurirServer.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ODataController
    {
        IDeliveryRepository deliveryRepository;
        IGeneralRepository generalRepository;
        IUserRepository userRepository;
        IMapper mapper;
        public DeliveriesController
        (IGeneralRepository generalRepository, IMapper mapper,IDeliveryRepository deliveryRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.deliveryRepository = deliveryRepository;
            this.generalRepository = generalRepository;
            this.mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        [Route("ODataGet")]
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
        [Route("StatisticsCouriers/{d11}/{d22}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatisticsModel>>> GetDeliveriesForStatisticsOfCourier(string d11,string d22)
        {
            try
            {
                DateTime d1 = new DateTime(Convert.ToInt64(d11));
                DateTime d2 = new DateTime(Convert.ToInt64(d22));
                List<StatisticsModel> list = new List<StatisticsModel>();
                var users = userRepository.GetUsersIdsWithCourierRole();
                foreach (var item in users)
                {
                    StatisticsModel sm = new StatisticsModel { UserID = item };
                    var user =await userRepository.GetUserAsync(item);
                    sm.ImePrezime = user.FirstName + " " + user.LastName;

                    sm.Procenat = user.Procenat;
                    var deliveries = deliveryRepository.GetAllDeliveries().Where(x => (x.EndTime > x.StartTime)&&(x.CourierID==item) && (x.EndTime.Date >= d1.Date) && (x.EndTime.Date <= d2.Date));
                    foreach (var delivery in deliveries)
                    {

                        sm.Promet += Convert.ToDouble(delivery.DeliveryPrice);
                        switch (delivery.PaymentTypeID)
                        {
                            //kes
                            case 1:
                                sm.PrometCash +=delivery.DeliveryPrice;
                                break;
                            //kupon
                            case 2:
                                sm.PrometFaktura += delivery.DeliveryPrice;
                                break;
                            case 3:
                                sm.PrometCupon += delivery.DeliveryPrice;
                                break;
                            default:
                                break;
                        }
                       
                        
                        
    

                    }
                    sm.BrojDostava = deliveries.Count();
                    sm.PrihodOdPrometa = sm.Promet * sm.Procenat/100;
                    list.Add(sm);

                }
               
                if (list != null)
                    return Ok(list);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("StatisticsDispatchers/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<StatisticsModel>> GetDeliveriesForStatisticsOfDispatcher(string d11, string d22)
        {
            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));
            try
            {
                List<StatisticsModel> list = new List<StatisticsModel>();
                var users = userRepository.GetUsersIdsWithDispatcherRole();
                foreach (var item in users)
                {
                    StatisticsModel sm = new StatisticsModel { UserID = item };
                    var deliveries = deliveryRepository.GetAllDeliveries().Where(x => (x.EndTime > x.StartTime) && (x.DispatcherID == item) && (x.EndTime.Date >= d1.Date) && (x.EndTime.Date <= d2.Date));

                    foreach (var delivery in deliveries)
                    {
                        sm.Promet += Convert.ToDouble(delivery.DeliveryPrice);
                        switch (delivery.PaymentTypeID)
                        {
                            //kes
                            case 1:
                                sm.PrometCash += delivery.DeliveryPrice;
                                break;
                            //kupon
                            case 2:
                                sm.PrometFaktura += delivery.DeliveryPrice;
                                break;
                            case 3:
                                sm.PrometCupon += delivery.DeliveryPrice;
                                break;
                            default:
                                break;
                        }

                    }
                    sm.BrojDostava = deliveries.Count();
                    sm.PrihodOdPrometa = deliveries.Count()*25;
                    list.Add(sm);

                }

                if (list != null)
                    return Ok(list);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("Delete/{DeliveryID}")]
        [HttpDelete]
        public ActionResult<string> DeleteDelivery(int DeliveryID = 0)
        {
            try
            {
                var del = deliveryRepository.GetByID(DeliveryID);
                if (del != null)
                    generalRepository.Delete<Delivery>(del);
                return Ok("delivery "+DeliveryID+" Deleted");
            }
            catch (Exception ex) { return NotFound("Delivery by id "+DeliveryID+" not found."); }
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