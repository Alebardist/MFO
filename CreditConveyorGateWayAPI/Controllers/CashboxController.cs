using Microsoft.AspNetCore.Authorization;
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashboxController : ControllerBase
    {
        private readonly ILogger<MFOController> _logger;
        private readonly IConfiguration _configiration;

        public CashboxController(ILogger<MFOController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configiration = configuration;
        }

        [HttpGet]
        [Route("/Balances")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult GetBalances()
        {
            //TODO: NotImplementedException
            //send RPC to CashboxService
            //return JSON with balances
            throw new NotImplementedException();
        }

    }
}
