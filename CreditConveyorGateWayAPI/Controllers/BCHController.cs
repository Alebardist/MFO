using BCHGrpcService;

using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Serilog;

using System;

namespace GatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BCHController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public BCHController(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns credit rating by credit parameters
        /// </summary>
        /// <param name="passport">
        /// JSON DTO CreditParameters
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/[Controller]/Rating")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetRating
        (
            [FromHeader(Name = "PassportNumbers")]
            string passport
        )
        {
            IActionResult result;

            try
            {
                var channel = GrpcChannel.ForAddress(_configuration.GetSection("BCHGrpcService:AddressAndPort").Value);
                var client = new BCHGrpc.BCHGrpcClient(channel);

                int creditRating = client.GetRatingByPassport(new RatingRequest { PassportNumber = passport }).Rating;

                result = Ok(creditRating);
            }
            catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                result = NotFound($"{passport} not found");
            }
            catch (RpcException e)
            {
                _logger.Error(e, e.Message);

                result = Problem($"{Grpc.Core.StatusCode.Internal}, {e.Message}");
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                result = Problem(e.Message, statusCode: 500);
            }

            return result;
        }

        /// <summary>
        /// Returns CreditHistory by passport from BCH db
        /// </summary>
        /// <param name="passport"></param>
        /// <returns>Json</returns>
        [HttpGet]
        [Route("/api/[Controller]/CreditHistory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        public IActionResult GetCreditHistoryByPassport([FromHeader(Name = "PassportNumbers")] string passport)
        {
            IActionResult result;

            try
            {
                using var channel = GrpcChannel.ForAddress(_configuration.GetSection("BCHGrpcService:AddressAndPort").Value);
                var client = new BCHGrpc.BCHGrpcClient(channel);

                var request = new CreditHistoryRequest()
                {
                    PassportNumber = passport
                };

                result = Ok(client.GetCreditHistory(request));
            }
            catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                result = NotFound($"{passport} not found");
            }
            catch (RpcException e)
            {
                _logger.Error(e, e.Message);

                result = Problem($"{Grpc.Core.StatusCode.Internal}, {e.Message}");
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                result = Problem(e.Message);
            }

            return result;
        }

    }
}
