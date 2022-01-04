using System;
using System.Collections.Generic;

using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Serilog;

using SharedLib;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CashboxController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly GrpcChannel _channel;
        private readonly Cashbox.CashboxClient _cashboxClient;
        public CashboxController(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _cashboxClient = new Cashbox.CashboxClient(_channel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/[controller]/Token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
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
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
            {
                result = Unauthorized(ex.Message);
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
        [ProducesResponseType(500)]
        public IActionResult GetBalances([FromHeader(Name = "Authorization")] string token)
        {
            IActionResult result;

            Metadata headers = new();
            headers.Add("Authorization", $"{token}");

            IEnumerable<BalanceObject> content;
            try
            {
                content = _cashboxClient.GetBalances(new Empty(), headers).Balances;
                result = Ok(content);
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
            {
                _logger.Warning(ex, ex.Message);
                result = Unauthorized(ex.Message);
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