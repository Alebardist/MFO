using System;
using System.Threading.Tasks;

using CalculationLib;

using Grpc.Core;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using Newtonsoft.Json;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

namespace BCHGrpcService.Services
{
    public class BCHGrpcService : BCHGrpc.BCHGrpcBase
    {
        private readonly ILogger<BCHGrpcService> _logger;

        public BCHGrpcService(ILogger<BCHGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<RatingReply> GetRatingByPassport(RatingRequest request, ServerCallContext context)
        {
            var clientCredits = MongoDBAccessor<Client>.
                                GetMongoCollection("BCH", "Clients").
                                Find(x => x.Passport == request.PassportNumber).
                                First().CreditHistory;

            var response = new RatingReply
            {
                Rating = CreditCalculations.GetCreditRating(clientCredits)
            };

            return Task.FromResult(response);
        }

        public override Task<CreditHistoryReply> GetCreditHistory(CreditHistoryRequest request, ServerCallContext context)
        {
            object creditHistories;
            try
            {
                creditHistories = MongoDBAccessor<Client>.GetMongoCollection("BCH", "Clients")
                    .Find(x => x.Passport == request.PassportNumbers)
                    .First().CreditHistory;
            }
            catch (InvalidOperationException e)
            {
                //TODO: check if this works properly, when given not found passport
                return Task.FromException<CreditHistoryReply>(e);
            }

            var reply = new CreditHistoryReply() { CreditHistoryJSON = JsonConvert.SerializeObject(creditHistories) };
            
            return Task.FromResult(reply);
        }
    }
}