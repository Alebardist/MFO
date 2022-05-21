using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Serilog;

using SharedLib.DTO.Application;

using System.Text;
using System;
using RabbitMQ.Client;

using Newtonsoft.Json;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ApplicationsController(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/api/[controller]/CreateNewApplication")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult CreateNewApplication([FromBody(EmptyBodyBehavior =
            Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Disallow)]
            CreditApplication creditApplication)
        {
            var factory = new ConnectionFactory() { HostName = _configuration.GetSection("RabbitMQ:Host").Value };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(creditApplication));

                channel.BasicPublish(exchange: _configuration.GetSection("RabbitMQ:ExchangeName").Value,
                                     routingKey: _configuration.GetSection("RabbitMQ:RoutingKey").Value,
                                     basicProperties: null,
                                     body: body);
            }

            CreditApplication newOne = creditApplication;

            return Created("/CreateNewApplication", creditApplication);
        }
    }
}