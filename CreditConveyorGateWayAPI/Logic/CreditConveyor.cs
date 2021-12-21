using System;

using BCHGrpcService;

using Grpc.Net.Client;

using MongoDB.Bson;
using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

namespace GatewayAPI.Logic
{
    public static class CreditConveyor
    {
        public static int GetCreditRating(string passport)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new BCHGrpc.BCHGrpcClient(channel);

            var response = client.GetRatingByPassport(new RatingRequest { PassportNumber = passport });

            return response.Rating;
        }

        public static Debt Delete(ObjectId id)
        {
            return MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").FindOneAndDelete(x => x.Id == id);
        }

        public static void SendMoneyToClient()
        {
            throw new NotImplementedException();
        }
    }
}
