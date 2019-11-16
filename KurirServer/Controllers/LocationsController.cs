using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KurirServer.Entities;
using KurirServer.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private IGeneralRepository generalRepository;
        private ILocationRepository locationRepository;
        private KurirDbContext context;
        public LocationsController(KurirDbContext context, ILocationRepository locationRepository, IGeneralRepository generalRepository)
        {
            this.generalRepository = generalRepository;
            this.locationRepository = locationRepository;
            this.context = context;
        }
        //AddLocation
        [Route("AddLocation")]
        [HttpPost]
        public async Task<ActionResult<string>> AddLocation(Location location)
        {
            try
            {
                //var l = locationRepository.GetLocationByUserID(location.UserID);
                //if (l != null)
                //{
                //    if(await generalRepository.Update(location))
                //    {
                //        return Ok(location);
                //    }
                //    else
                //    {
                //        return StatusCode(StatusCodes.Status500InternalServerError);
                //    }
                //}
                //else
                //{
                    if (await locationRepository.AddLocation(location))
                    {
                        return Ok(location);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                //}
                

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [Route("GetLocationByID/{ID}")]
        [HttpGet]
        public async Task<ActionResult<string>> GetLocationByID(int ID)
        {
            try
            {
                Location l = await locationRepository.GetLocationByID(ID);
                if (l != null)
                    return Ok(JsonConvert.SerializeObject(l));
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
           
        }
        [Route("GetLocationByUserID/{ID}")]
        [HttpGet]
        public async Task<ActionResult<string>> GetLocationByUserID(int ID)
        {
            try
            {
                Location l = await locationRepository.GetLocationByUserID(ID);
                if (l != null)
                    return Ok(JsonConvert.SerializeObject(l));
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }

}