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
        IGeneralRepository generalRepository;
        IMapper mapper;
        public DeliveriesController(IGeneralRepository generalRepository, IMapper mapper)
        {
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
    }
}