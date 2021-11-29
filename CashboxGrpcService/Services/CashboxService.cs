using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        [Authorize]
        public override Task<SendMoneyReply> SendMoney(SendMoneyRequest request, ServerCallContext context)
        {
            SendMoneyReply reply;
            reply = new SendMoneyReply()
            {
                ErrorMessage = $"",
                Result = SendMoneyReply.Types.operationResult.Ok
            };

            return Task.FromResult(new SendMoneyReply());
        }

        [AllowAnonymous]
        public override Task<LogInReply> LogInService(LogInRequest request, ServerCallContext context)
        {
            var reply = new LogInReply() { Result = LogInReply.Types.loginResult.WrongUserNameOrUserPassword };

            if (CheckCredentialsCorrectness(request))
            {
                var claims = new List<Claim>()
                {
                new Claim(JwtRegisteredClaimNames.Sub, request.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now).AddDays(1).ToString())
                };

                var key = new SymmetricSecurityKey(new ASCIIEncoding().GetBytes(_configuration.GetSection("key").Value));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken("issuer", "audience", claims, 
                    DateTime.Now,
                    DateTime.Now.AddMinutes(5),
                    cred
                    );

                reply.Token = new JwtSecurityTokenHandler().WriteToken(token);
                reply.Result = LogInReply.Types.loginResult.Success;
            }

            Console.WriteLine(reply.Token);
            return Task.FromResult(reply);
        }

        private bool CheckCredentialsCorrectness(LogInRequest request)
        {
            byte[] passwordHash = SHA256.HashData(new ASCIIEncoding().GetBytes(request.UserPassword));

            var filter = Builders<UserCredentials>.Filter.Where(x => x.UserName == request.UserName
                                                                &&
                                                                x.UserPassword == passwordHash);

            return new MongoDBAccessor<UserCredentials>(_configuration.GetSection("MongoDB:DBName").Value,
                                                        _configuration.GetSection("MongoDB:CollectionName").Value)
                                                        .ReadWithFilter(filter).Any();
        }
    }
}