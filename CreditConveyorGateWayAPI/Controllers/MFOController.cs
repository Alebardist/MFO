using System;
using System.Diagnostics;

using BCHGrpcService;

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
        [ProducesDefaultResponseType(typeof(JsonResult))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetRating
        (
            [FromHeader(Name = "PassportNumbers")]
            string passport
        )
        {
            JsonResult result = new("") { ContentType = "application/json" };

            try
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new BCHGrpc.BCHGrpcClient(channel);

                int creditRating = client.GetRatingByPassport(new RatingRequest { PassportNumber = passport }).Rating;

                result = new JsonResult(creditRating) { StatusCode = 200, ContentType = "application/json" };
            }
            catch (Exception ex)
            {
                result.Value = ex.Message;
                result.StatusCode = 500;

                Debug.WriteLine(ex.Message);
            }

            return new JsonResult(result);
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
        string debtDTO
        )
        {
            StatusCodeResult result = new(404);

            //Deserealize DTO from body into object
            Debt updatedDebt = JsonConvert.DeserializeObject<Debt>(debtDTO);
            //find document by creditDTO.Id
            //Create UpdateDefinition fields with new DTO from body
            var filter = Builders<Debt>.Filter.Eq("_id", updatedDebt.Id);
            var update = Builders<Debt>.Update.Set("Passport", updatedDebt.Passport).
                                                        Set("Loan", updatedDebt.Loan).
                                                        Set("Issued", updatedDebt.Issued).
                                                        Set("OverdueInDays", updatedDebt.OverdueInDays).
                                                        Set("Penalty", updatedDebt.Penalty).
                                                        Set("Interest", updatedDebt.Interest);
            if (MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").FindOneAndUpdate<Debt>(filter, update) != null)
            {
                result = new(200);
            } 

            return result;
        }

        //TODO: take PassportNUmbers from route, before this put together numbers in db
        /// <summary>
        /// Returns CreditHistory by passport from BCH db
        /// </summary>
        /// <param name="passport"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("/api/[Controller]/CreditHistory")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        public JsonResult GetCreditHistoryByPassport([FromHeader(Name = "PassportNumbers")] string passport)
        {
            object result = new ObjectResult("");
            int statusCode = 200;

            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new BCHGrpc.BCHGrpcClient(channel);

                var request = new CreditHistoryRequest()
                {
                    PassportNumbers = passport
                };

                result = client.GetCreditHistory(request).CreditHistoryJSON;

                //TODO: process result when it contains Exception

                statusCode = 200;
            }
            catch (InvalidOperationException e)
            {
                statusCode = 404;
                result = e.Message + $" {passport}";
            }
            catch (Exception e)
            {
                statusCode = 500;
                result = $"Internal server error " + e.Message;
            }

            return new JsonResult(result) { StatusCode = statusCode };
        }

        //TODO: unimplemented
        [HttpGet]
        [Route("/api/[Controller]/{debtId}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public JsonResult GetinformationByDebtId([FromRoute(Name = "debtId")] string debtId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("/api/[Controller]/{debtId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public StatusCodeResult DeleteDebtByDebtId([FromRoute(Name = "debtId")] string debtId)
        {
            StatusCodeResult statusCode = new(500);

            var result = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").FindOneAndDelete(x => x.Id == new ObjectId(debtId));

            if (result != null)
            {
                statusCode = new StatusCodeResult(200);
            }
            else
            {
                statusCode = new StatusCodeResult(404);
            }

            return statusCode;
        }
    }
}