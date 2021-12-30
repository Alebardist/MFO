using System;
using System.Diagnostics;

using BCHGrpcService;

using Grpc.Core;
using Grpc.Net.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MongoDB.Bson;
using MongoDB.Driver;

using Newtonsoft.Json;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class MFOController : ControllerBase
    {
        private readonly ILogger<MFOController> _logger;

        public MFOController(ILogger<MFOController> logger)
        {
            _logger = logger;
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
            catch (Exception ex)
            {
                result = Problem(ex.Message, statusCode: 500);
            }

            return result;
        }

        [HttpPut]
        [Route("/api/[Controller]/{creditNoteId}")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCreditInformation
        (
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Disallow)]
        Debt updatedDebt,
        [FromRoute(Name = "creditNoteId")]
        string creditNoteId
        )
        {
            IActionResult result;

            try
            {
                updatedDebt.Id = ObjectId.Parse(creditNoteId);

                var updateResult = MongoDBAccessor<Debt>.
                    GetMongoCollection("MFO", "Debts").
                    ReplaceOne(x => x.Id == ObjectId.Parse(creditNoteId), updatedDebt);

                if (updateResult.ModifiedCount == 1)
                {
                    result = Ok();
                }
                else
                {
                    result = NotFound(creditNoteId);
                }
            }
            catch (Exception e)
            {
                result = Problem(e.Message, statusCode: 500);
                //TODO: logging
            }

            return result;
        }

        //TODO: take PassportNUmbers from route, before this put together numbers in db
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
                    PassportNumbers = passport
                };
                
                result = Ok(client.GetCreditHistory(request));
            }
            catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                result = NotFound($"{passport} not found");
            }
            catch (RpcException e)
            {
                result = Problem($"{Grpc.Core.StatusCode.NotFound}, {e.Message}");
            }
            catch (Exception e)
            {
                result = Problem(e.Message);
            }

            return result;
        }

        //TODO: unimplemented
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
                var result = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debt").Find(x => x.Id == ObjectId.Parse(debtId)).First();
                reply = Ok(result);
            }
            catch (InvalidOperationException e)
            {
                reply = NotFound($"{debtId} not exists");
            }
            catch (Exception e)
            {
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
            IActionResult result;
            var deletionResult = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").DeleteOne(x => x.Id == new ObjectId(debtId));

            if (deletionResult.DeletedCount == 1)
            {
                result = Ok();
            }
            else
            {
                result = NotFound(debtId);
            }

            return result;
        }
    }
}