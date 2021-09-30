using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditConveyorGateWayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly ILogger<CreditController> _logger;

        public CreditController(ILogger<CreditController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public JsonResult ApproveCredit()
        {
            JsonResult result = new JsonResult("");
            try
            {
                throw new Exception("error");
            }
            catch (Exception ex)
            {
                result = new JsonResult(ex.Message);
            }


            return result;
        }
    }
}
