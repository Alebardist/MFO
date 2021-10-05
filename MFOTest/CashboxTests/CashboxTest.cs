using CashboxGrpcService;
using Grpc.Net.Client;
using Xunit;

namespace MFOTest.CashboxTests
{
    public class CashboxTest
    {
        private GrpcChannel _channel = GrpcChannel.ForAddress("https://localhost:5001");
        private Cashbox.CashboxClient _cashboxClient;

        public CashboxTest()
        {
            _cashboxClient = new Cashbox.CashboxClient(_channel);
        }

        [Fact]
        public void SendMoneyReplyMustBeOK()
        {
            var expected = SendMoneyReply.Types.operationResult.Ok;
            var request = new SendMoneyRequest()
            {
                CardNumber = "1234 1234 1234 1234"
            };

            var actual = _cashboxClient.SendMoney(request).Result;

            Assert.Equal(expected, actual);
        }
    }
}
