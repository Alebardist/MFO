using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Serilog;

using SharedLib;

using System;
using System.Collections.Generic;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CashboxController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly Cashbox.CashboxClient _cashboxClient;

        public CashboxController(ILogger logger, IConfiguration configuration, Cashbox.CashboxClient cashboxClient)
        {
            _logger = logger;
            _configuration = configuration;
            _cashboxClient = cashboxClient;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/[controller]/Token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(429)]
        [ProducesResponseType(500)]
        public IActionResult GetToken(
            [FromHeader(Name = "UserName")]
            string name,
            [FromHeader(Name = "Password")]
            string password
            )
        {
            ActionResult result = Unauthorized("Wrong user name or password");

            try
            {
                if (SharedFuncs.CheckCredentialsCorrectness(name, password))
                {
                    var token = SharedFuncs.GenerateToken(name, "admin",
                        new TokenParameters(_configuration.GetSection("JWT:key").Value,
                                            _configuration.GetSection("JWT:validIssuer").Value,
                                            _configuration.GetSection("JWT:validAudience").Value));
                    result = Ok(token);
                }
                else
                {
                    _logger.Debug($"Unauthorized {name}, {password}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result = Problem(ex.Message);
            }

            return result;
        }

        [HttpGet]
        [Authorize]
        [Route("/api/[controller]/Balances")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(429)]
        [ProducesResponseType(500)]
        public IActionResult GetBalances()
        {
            IActionResult result;

            IEnumerable<BalanceObject> content;
            try
            {
                content = _cashboxClient.GetBalances(new Empty()).Balances;
                result = Ok(content);
            }
            catch (RpcException ex)
            {
                _logger.Error(ex, ex.Message);
                result = Problem("RPC ex " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result = Problem(ex.Message);
            }

            return result;
        }
    }
}