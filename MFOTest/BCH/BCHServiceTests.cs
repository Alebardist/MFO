using BCHGrpcService;

using Grpc.Net.Client;

using Xunit;

namespace MFOTest.BCHService
{
    public class BCHServiceTests
    {
        [Fact]
        public async void RatingReplyMustContainExpectedRatingForRatingRequest()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new BCHGrpc.BCHGrpcClient(channel);

            var reply = await client.GetRatingByPassportAsync(
                              new RatingRequest
                              {
                                  PassportNumber = "1234 123456"
                              });

            Assert.Equal(25, reply.Rating);
        }
    }
}