using BCHGrpcService;

using Grpc.Core;

using Microsoft.AspNetCore.Mvc;

using Serilog;

using System;

namespace GatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BCHController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly BCHGrpc.BCHGrpcClient _client;

        public BCHController(ILogger logger, BCHGrpc.BCHGrpcClient client)
        {
            _logger = logger;
            _client = client;
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
                int creditRating = _client.GetRatingByPassport(new RatingRequest { PassportNumber = passport }).Rating;
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
                var request = new CreditHistoryRequest()
                {
                    PassportNumber = passport
                };

                result = Ok(_client.GetCreditHistory(request));
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
