using System.Threading.Tasks;

using CalculationLib;

using Grpc.Core;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

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
            //TODO: (2)
            //получение нужного клиента по паспорту
            var clientCredits = MongoDBAccessor<Client>.
                                GetMongoCollection("BCH", "Clients").
                                Find(x => x.Passport == request.PassportNumber).
                                First().CreditHistory;
            //вычисление рейтинга по КИ полученного клиента
            var response = new RatingReply
            {
                Rating = CreditCalculations.GetCreditRating(clientCredits)
            };

            return Task.FromResult(response);
        }
    }
}