using Grpc.Core;

using System.Threading.Tasks;

using Serilog;

namespace BCHGrpcService.Services
{
    public class BCHGrpcService : BCHGrpc.BCHGrpcBase
    {
        private readonly ILogger _logger;

        public BCHGrpcService(ILogger logger)
        {
            _logger = logger;
        }

        public override Task<RatingReply> GetRatingByPassport(RatingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RatingReply
            {
                Rating = 1f
            });
        }
    }
}
