using System;
using System.Net.Http;

using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using Xunit;

namespace MFOTest.CashboxServiceTests
{
    public class CashboxServiceTests
    {
        private readonly GrpcChannel _channel;
        private readonly Cashbox.CashboxClient _cashboxClient;
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44317/api/Cashbox") };

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