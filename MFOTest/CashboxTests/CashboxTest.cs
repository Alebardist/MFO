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
        public void MyTestMethod()
        {
            var expected = "OK";
            var request = new SendMoneyRequest()
            {
                CardNumber = "1234 1234 1234 1234"
            };

            var actual = _cashboxClient.SendMoney(request).OperationResult;

            Assert.Equal(expected, actual);
        }
    }
}
