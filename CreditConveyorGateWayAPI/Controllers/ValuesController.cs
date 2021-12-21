using Microsoft.AspNetCore.Authorization;
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<MFOController> _logger;

        public ValuesController(ILogger<MFOController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/Method")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult Method
        (
            [FromHeader(Name = "Arg")]
            string arg
        )
        {
            //TODO: NotImplementedException
            throw new NotImplementedException();
        }

    }
}
