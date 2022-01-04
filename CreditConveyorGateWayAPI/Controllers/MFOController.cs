using System;

using BCHGrpcService;

using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Serilog;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class MFOController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MFOController(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/[Controller]/Token")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public IActionResult GetToken
        (
            [FromHeader(Name = "Login")]
            string login,
            [FromHeader(Name = "Password")]
            string password
        )
        {
            //TODO: NotImplementedException
            //logging: time, token, client's IP
            throw new NotImplementedException();
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
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
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

        [HttpPut]
        [Route("/api/[Controller]")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCreditInformation
        (
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Disallow)]
        Debt updatedDebt
        )
        {
            IActionResult result;

            try
            {
                var updateResult = MongoDBAccessor<Debt>.
                    GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                        _configuration.GetSection("MongoDB:CollectionName").Value).
                    ReplaceOne(x => x.Id == updatedDebt.Id, updatedDebt);

                result = (updateResult.ModifiedCount == 1) ? Ok() : NotFound(updatedDebt.Id);
            }
            catch (FormatException e)
            {
                result = BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                result = Problem(e.Message, statusCode: 500);
            }

            return result;
        }

        //TODO: take PassportNumber from route, before this put together numbers in db
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
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
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

        [HttpGet]
        [Route("/api/[Controller]/{debtId}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetinformationByDebtId([FromRoute(Name = "debtId")] string debtId)
        {
            ObjectResult reply;

            try
            {
                var result = MongoDBAccessor<Debt>.
                    GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                        _configuration.GetSection("MongoDB:CollectionName").Value).
                                        Find(x => x.Id == debtId).First();
                reply = Ok(result);
            }
            catch (InvalidOperationException)
            {
                reply = NotFound($"{debtId} not exists");
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                reply = Problem(e.Message, statusCode: 500);
            }

            return reply;
        }

        [HttpDelete]
        [Route("/api/[Controller]/{debtId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult DeleteDebtByDebtId([FromRoute(Name = "debtId")] string debtId)
        {
            var deletionResult = MongoDBAccessor<Debt>.
                GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                    _configuration.GetSection("MongoDB:CollectionName").Value).
                                    DeleteOne(x => x.Id == debtId);

            return deletionResult.DeletedCount == 1 ? Ok() : NotFound(debtId);
        }
    }
}