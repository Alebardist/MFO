using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Serilog;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System;

namespace GatewayAPI.Controllers
{
    [ApiController]
    [Route("/api/[Controller]")]
    public class MFOController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public MFOController(ILogger logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
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
            //UNDONE: NotImplementedException
            //TODO:logging: time, token, client's IP
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("/api/[Controller]/Debt")]
        [Consumes("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
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
                Debt debtDTO = new();

                if (!_memoryCache.TryGetValue(debtId, out debtDTO))
                {
                    debtDTO = MongoDBAccessor<Debt>.
                    GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                        _configuration.GetSection("MongoDB:CollectionName").Value).
                                        Find(x => x.Id == debtId).First();

                    _memoryCache.Set(debtId, debtDTO, TimeSpan.FromMinutes(5));
                }

                reply = Ok(debtDTO);
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

        [HttpGet]
        [Route("/checkHealth")]
        public IActionResult Checkstatus()
        {
            return Ok("Health Ok!");
        }
    }
}