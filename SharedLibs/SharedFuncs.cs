using Microsoft.IdentityModel.Tokens;

using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SharedLib
{
    public static class SharedFuncs
    {
        public static bool CheckCredentialsCorrectness(string userName, string password,
        string dbName = "MFO", string collectionName = "UsersCredentials")
        {
            byte[] passwordHash = SHA256.HashData(new ASCIIEncoding().GetBytes(password));

            try
            {
                MongoDBAccessor<UserCredentials>.
                GetMongoCollection(dbName, collectionName).
                                   Find(x => x.UserName == userName
                                        &&
                                        x.UserPassword == passwordHash).First();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Credentials not found {e.Message}");
                return false;
            }

            return true;
        }

        public static string GenerateToken(string actor, string role, TokenParameters tokenParameters)
        {
            var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Actor, actor),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
                };

            var key = new SymmetricSecurityKey(new ASCIIEncoding().GetBytes(tokenParameters.SecretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: tokenParameters.ValidIssuer,
                                            audience: tokenParameters.ValidAudience,
                                            claims,
                                            DateTime.Now,
                                            DateTime.Now.AddMinutes(5),
                                            signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}