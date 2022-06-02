using BCHGrpcService;

namespace MFOTest.BCHService
{
    public class BCHServiceTests
    {
        private readonly GrpcChannel _channel = GrpcChannel.ForAddress("https://localhost:5003");
        private readonly BCHGrpc.BCHGrpcClient _client;

        public BCHServiceTests()
        {
            _client = new BCHGrpc.BCHGrpcClient(_channel);
        }

        [Fact]
        public async void RatingReplyMustContainExpectedRatingForRatingRequest()
        {
            var reply = await _client.GetRatingByPassportAsync(
                              new RatingRequest
                              {
                                  PassportNumber = "1234 123456"
                              });

            Assert.Equal(25, reply.Rating);
        }

        [Fact]
        public async void RatingReplyMustThrowNotFound()
        {
            async Task Meth()
            {
                await _client.GetRatingByPassportAsync(
                                  new RatingRequest
                                  {
                                      PassportNumber = "1234 123450"
                                  });
            }

            await Assert.ThrowsAsync<RpcException>(async () => await Meth());
        }
    }
}