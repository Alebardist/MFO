using System;

using BCHGrpcService;

using Grpc.Net.Client;
using SharedLib.DTO;

namespace CreditConveyorGateWayAPI.Logic
{
    public class CreditConveyor
    {
        public int GetCreditRating(CreditParameters creditParameters)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new BCHGrpc.BCHGrpcClient(channel);

            var response = client.GetRatingByPassport(new RatingRequest { PassportNumber = creditParameters.PassportNumber });

            return response.Rating;
        }

        public void SendMoneyToClient()
        {
            throw new NotImplementedException();
        }

    }
}
