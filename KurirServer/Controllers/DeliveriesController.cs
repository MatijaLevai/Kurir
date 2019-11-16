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
        IAddressRepository addressRepository;
        IMapper mapper;
        public DeliveriesController
        (IGeneralRepository generalRepository, IMapper mapper,IDeliveryRepository deliveryRepository, IUserRepository userRepository,IAddressRepository addressRepository)
        {
            this.addressRepository = addressRepository;
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
                var x = await generalRepository.SaveChangesAsync();
                if(x)
                return Ok(newDelivery);
                else return StatusCode(StatusCodes.Status500InternalServerError);
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
        [Route("GetDelivery/{id}")]
        [HttpGet]
        public ActionResult<Delivery> GetDelivery(int id)
        {
            var d = (Delivery)deliveryRepository.GetByID(id);
            if (d != null)
                return Ok(d);
            else return NotFound(null);
        }
        [EnableQuery]
        [Route("GetDeliveries")]
        [HttpGet]
        public IEnumerable<Delivery> GetDeliveries()
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
        [EnableQuery]
        [Route("GetTodaysDeliveriesForCourier/{Courierid}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetTodaysDeliveriesForCourier(int Courierid)
        {
            try
            {

                var deliveries = deliveryRepository.GetTodaysDeliveriesAsCourier(Courierid);

                if (deliveries != null)
                {
                    List<Delivery> list = new List<Delivery>(deliveries);
                        foreach (var item in list)
                    {
                        item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                        item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                    }
                    return Ok(deliveries);
                }
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
                    var deliveries = deliveryRepository.GetAllDeliveries().Where(x => (x.DeliveryStatus == 4) && (x.CourierID == item) && (x.CreateTime.Date >= d1.Date) && (x.CreateTime.Date <= d2.Date));

                    foreach (var delivery in deliveries)
                    {

                        sm.Promet += Convert.ToDouble(delivery.DeliveryPrice);
                        switch (delivery.PaymentTypeID)
                        {
                            //kes
                            case 1:
                                sm.PrometCash +=delivery.DeliveryPrice;
                                break;
                            //faktura
                            case 2:
                                sm.PrometFaktura += delivery.DeliveryPrice;
                                break;
                            //kupon
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
                    var deliveries = deliveryRepository.GetAllDeliveries().Where(x => (x.DeliveryStatus == 4) && (x.DispatcherID == item) && (x.CreateTime.Date >= d1.Date) && (x.CreateTime.Date <= d2.Date));

                    foreach (var delivery in deliveries)
                    {
                        sm.Promet += Convert.ToDouble(delivery.DeliveryPrice);
                        switch (delivery.PaymentTypeID)
                        {
                            //kes
                            case 1:
                                sm.PrometCash += delivery.DeliveryPrice;
                                break;
                            //faktura odnosno preko racuna
                            case 2:
                                sm.PrometFaktura += delivery.DeliveryPrice;
                                break;
                            //kupon
                            case 3:
                                sm.PrometCupon += delivery.DeliveryPrice;
                                break;
                            default:
                                break;
                        }

                    }
                    sm.BrojDostava = deliveries.Count();
                    sm.PrihodOdPrometa = sm.BrojDostava * 25;
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
        [Route("GetByDate/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetDeliveriesByDate(string d11, string d22)
        {
            
            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));
           
            List<Delivery> deliveries = new List<Delivery>(deliveryRepository.GetDeliveriesByDate(d1, d2));

                if (deliveries != null)
                {
                    List<Delivery> list = new List<Delivery>(deliveries);
                    foreach (var item in list)
                    {
                        item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                        item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                    }
                    return Ok(deliveries);
                }
                else return NotFound();
        }
        //GetDeliveriesByDateCourirDispatch
        [Route("GetDeliveriesByDateCourierDispatch/{id}/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetDeliveriesByDateCourierDispatch(int id,string d11, string d22)
        {

            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));

            List<Delivery> deliveries = new List<Delivery>(deliveryRepository.GetDeliveriesByDateCourierAndDispatch(d1, d2,id));

            if (deliveries != null)
            {
                List<Delivery> list = new List<Delivery>(deliveries);
                foreach (var item in list)
                {
                    item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                    item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                }
                return Ok(deliveries);
            }
            else return NotFound();
        }
        [Route("GetByDateAndUser/{id}/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetByDateAndUser(int id,string d11, string d22)
        {

            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));

            List<Delivery> deliveries = new List<Delivery>(deliveryRepository.GetDeliveriesByDateUserID(d1, d2,id));

            if (deliveries != null)
            {
                List<Delivery> list = new List<Delivery>(deliveries);
                foreach (var item in list)
                {
                    item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                    item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                }
                return Ok(deliveries);
            }
                else return NotFound();
        }
        [Route("GetByDateAndCourier/{id}/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetByDateAndCourier(int id, string d11, string d22)
        {

            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));

            List<Delivery> deliveries = new List<Delivery>(deliveryRepository.GetDeliveriesByDateCourierID(d1, d2, id));

            if (deliveries != null)
            {
                List<Delivery> list = new List<Delivery>(deliveries);
                foreach (var item in list)
                {
                    item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                    item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                }
                return Ok(deliveries);
            }
            else return NotFound();
        }
        [Route("GetByDateAndDispatcher/{id}/{d11}/{d22}")]
        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetByDateAndDispatcher(int id, string d11, string d22)
        {

            DateTime d1 = new DateTime(Convert.ToInt64(d11));
            DateTime d2 = new DateTime(Convert.ToInt64(d22));

            List<Delivery> deliveries = new List<Delivery>(deliveryRepository.GetDeliveriesByDateDispatcherID(d1, d2, id));

            if (deliveries != null)
            {
                List<Delivery> list = new List<Delivery>(deliveries);
                foreach (var item in list)
                {
                    item.StartAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.StartAddressID));
                    item.EndAddress = addressRepository.GetAddressByID(Convert.ToInt32(item.EndAddressID));
                }
                return Ok(deliveries);
            }
            else return NotFound();
        }
        [Route("Delete/{DeliveryID}")]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteDelivery(int DeliveryID = 0)
        {
            if(DeliveryID>0)
            {
                var del = deliveryRepository.GetByID(DeliveryID);
                if (del != null)
                { generalRepository.Delete<Delivery>(del);
                    if(await generalRepository.SaveChangesAsync())
                        return Ok("Dostava pod brojem " + DeliveryID + " je obrisana");
                    else
                        return NotFound("Brisanje dostave pod brojem  " + DeliveryID + " nije uspelo.");
                }
                else
                    return NotFound("Dostava pod brojem  " + DeliveryID + " nije pronađena."); 
            }
            else
                return NotFound("Dostava pod brojem  " + DeliveryID + " nije pronađena.");

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