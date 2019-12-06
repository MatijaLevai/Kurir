using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaraBaraController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "BaraBara";
        }

    }
}
