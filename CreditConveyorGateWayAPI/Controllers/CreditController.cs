using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;

using Newtonsoft.Json;
using SharedLib.DTO;
using System.Diagnostics;
using CreditConveyorGateWayAPI.Logic;

namespace CreditConveyorGateWayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly ILogger<CreditController> _logger;

        public CreditController(ILogger<CreditController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult ApproveCredit([FromBody] object creditParameters)
        {
            JsonResult result = new JsonResult("");
            
            try
            {
                CreditParameters creditParametersDTO = JsonConvert.DeserializeObject<CreditParameters>(creditParameters.ToString());
                int creditRating = new CreditConveyor().GetCreditRating(creditParametersDTO);
                result = new JsonResult(creditRating);
            }
            catch (JsonReaderException ex)
            {
                result.Value = ex.Message;
                result.StatusCode = 400;

                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                result.Value = ex.Message;
                result.StatusCode = 500;

                Debug.WriteLine(ex.Message);
            }

            return new JsonResult(result);
        }
    }
}