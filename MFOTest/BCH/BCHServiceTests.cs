using BCHGrpcService;

using Grpc.Net.Client;

using Xunit;

namespace MFOTest.BCHService
{
    public class BCHServiceTests
    {
        private GrpcChannel _channel = GrpcChannel.ForAddress("https://localhost:5003");

        [Fact]
        public async void RatingReplyMustContainExpectedRatingForRatingRequest()
        {
            var client = new BCHGrpc.BCHGrpcClient(_channel);

            var reply = await client.GetRatingByPassportAsync(
                              new RatingRequest
                              {
                                  PassportNumber = "1234 123456"
                              });

            Assert.Equal(25, reply.Rating);
        }
    }
}