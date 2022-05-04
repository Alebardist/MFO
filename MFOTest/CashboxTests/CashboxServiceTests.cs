using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Net.Client;

using Xunit;

namespace MFOTest.CashboxServiceTests
{
    public class CashboxServiceTests
    {
        private readonly GrpcChannel _channel;
        private readonly Cashbox.CashboxClient _cashboxClient;

        public CashboxServiceTests()
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _cashboxClient = new Cashbox.CashboxClient(_channel);
        }

        [Fact]
        public void GetBalancesMustReturnExpectedBalancesReply()
        {
            var actual = _cashboxClient.GetBalances(new Empty());

            Assert.NotEmpty(actual.Balances);
        }
    }
}