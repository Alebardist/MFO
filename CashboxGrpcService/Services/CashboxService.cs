using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

namespace CashboxGrpcService.Services
{
    public class CashboxService : Cashbox.CashboxBase
    {
        private readonly IConfiguration _configuration;

        public CashboxService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Log in service to get JWT. 
        /// Docs https://datatracker.ietf.org/doc/html/rfc7519
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>Reply with operation result and JWT</returns>
        [AllowAnonymous]
        public override Task<LogInReply> LogInService(LogInRequest request, ServerCallContext context)
        {
            var reply = new LogInReply() { Result = LogInReply.Types.loginResult.WrongUserNameOrPassword };
            
            if (CheckCredentialsCorrectness(request))
            {
                var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Actor, request.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "admin")
                };

                var key = new SymmetricSecurityKey(new ASCIIEncoding().GetBytes(_configuration.GetSection("JWT:key").Value));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(issuer: _configuration.GetSection("JWT:validIssuer").Value,
                                                audience: _configuration.GetSection("JWT:validAudience").Value,
                                                claims,
                                                DateTime.Now,
                                                DateTime.Now.AddMinutes(5),
                                                credentials);

                reply.Token = new JwtSecurityTokenHandler().WriteToken(token);
                reply.Result = LogInReply.Types.loginResult.Success;
            }

            Console.WriteLine(reply.Token);

            return Task.FromResult(reply);
        }

        //TODO: unimplemented
        [Authorize]
        public override Task<SendMoneyReply> SendMoney(SendMoneyRequest request, ServerCallContext context)
        {
            SendMoneyReply reply;
            reply = new SendMoneyReply()
            {
                ErrorMessage = $"",
                Result = SendMoneyReply.Types.operationResult.Error
            };

            return Task.FromResult(reply);
        }

        [Authorize]
        public override Task<BalancesReply> GetBalances(Empty request, ServerCallContext context)
        {
            BalancesReply reply;

            var balances = MongoDBAccessor<Balance>.
                GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                    _configuration.GetSection("MongoDB:BalancesCollectionName").Value).
                Find(FilterDefinition<Balance>.Empty).ToEnumerable();
            TranslateEnumerableToObjects(balances, out reply);

            return Task.FromResult(reply);
        }

        private bool CheckCredentialsCorrectness(LogInRequest request)
        {
            byte[] passwordHash = SHA256.HashData(new ASCIIEncoding().GetBytes(request.UserPassword));

            return MongoDBAccessor<UserCredentials>.
                GetMongoCollection(_configuration.GetSection("MongoDB:DBName").Value,
                                   _configuration.GetSection("MongoDB:CollectionName").Value).
                                   Find(x => x.UserName == request.UserName
                                        &&
                                        x.UserPassword == passwordHash).Any();
        }

        private void TranslateEnumerableToObjects(IEnumerable<Balance> balances, out BalancesReply reply)
        {
            reply = new BalancesReply();

            foreach (var obj in balances)
            {
                var protoObj = new BalanceObject()
                {
                    Id = obj.Id.ToString(),
                    Storage = obj.StorageName,
                    Balance = (float)obj.Money
                };
                reply.Balances.Add(protoObj);
            }
        }
    }
}