using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CreditConveyorGateWayAPI.Logic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using SharedLib.DTO;

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

        [HttpGet]
        public IActionResult GetToken()
        {
            return null;
        }

    }
}