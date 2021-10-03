using System;
using System.Diagnostics;

using BCHGrpcService;

using Grpc.Net.Client;

namespace CreditConveyorGateWayAPI.Logic
{
    public class CreditHistory
    {
        public float GetCreditRating()
        {
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            //var client = new BCHGrpcService.BCHGrpcServiceClient(channel);

            //var response = client.GetRatingByPassport(new RatingRequest { PassportNumber = "1234 123456" });

            return 1f;
        }
    }
}
