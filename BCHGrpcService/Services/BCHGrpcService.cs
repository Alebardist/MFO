using Grpc.Core;

using System.Threading.Tasks;
using System;

//using Serilog;
using Microsoft.Extensions.Logging;

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
            var response = new RatingReply
            {
                Rating = 76
            };
            
            return Task.FromResult(response);
        }
    }
}