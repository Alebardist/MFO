using System;
using System.Diagnostics;
using System.Linq;

using GatewayAPI.Logic;

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
        [Route("/Token")]
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
        [Route("/Rating")]
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
            JsonResult result = new JsonResult("") { ContentType = "application/json" };

            try
            {
                int creditRating = CreditConveyor.GetCreditRating(passport);
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
        [Route("/{creditNoteId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCreditInformation
        (
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Disallow)]
        object creditDTO
        )
        {
            //TODO: unimplemented
            //Deserealize dto from body into 
            //find document by creditDTO.Id
            //Create UpdateDefinition fields with new DTO from body
            throw new NotImplementedException();
            //return new StatusCodeResult(200);
        }

        //TODO: take PassportNUmbers from route, before this put together numbers in db
        /// <summary>
        /// Returns CreditHistory by passport from BCH db
        /// </summary>
        /// <param name="passport"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("/CreditHistory")]
        //[Route("[controller]/CreditsByPassport/{passportNumbers}")]
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
                //TODO: move this code to separate method (procedure in BCH service)
                result = MongoDBAccessor<Client>.GetMongoCollection("BCH", "Clients")
                    .Find(x => x.Passport == passport)
                    .First().CreditHistory;
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

        [HttpGet]
        [Route("/{creditNoteId}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public JsonResult GetinformationByCreditId([FromRoute(Name = "creditNoteId")] string debtId)
        {
            var info = new
            {
                Id = debtId,
                Name = "Belov Yuriy Alexeevich",
                creditSum = 6500,
                DateTaken = new DateTime(2021, 3, 23),
                Percent = 3.6m
            };

            return new JsonResult(JsonConvert.SerializeObject(info));
        }

        [HttpDelete]
        [Route("/{debtId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public StatusCodeResult DeleteinformationByCreditId([FromRoute(Name = "debtId")] string debtId)
        {
            StatusCodeResult statusCode = new StatusCodeResult(500);

            if (CreditConveyor.Delete(new ObjectId(debtId)) != null)
            {
                statusCode = new StatusCodeResult(200);
            }

            return new StatusCodeResult(200);
        }

    }
}