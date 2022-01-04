using System;
using System.Net.Http;

using CashboxGrpcService;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using Grpc.Net.Client;

using Xunit;

namespace MFOTests.CashboxTests
{
    public class CashboxServiceTest
    {
        private readonly GrpcChannel _channel;
        private readonly Cashbox.CashboxClient _cashboxClient;
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:44317/api/Cashbox") };

        public CashboxServiceTest()
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5001");
            _cashboxClient = new Cashbox.CashboxClient(_channel);
        }

        [Fact]
        public void GetBalancesMustReturnExpectedBalancesReply()
        {
            HttpContent contentWithHeaders = new StringContent(""); //TODO: use multipart instead of headers?
            contentWithHeaders.Headers.Add("UserName", "admin");
            contentWithHeaders.Headers.Add("Password", "adminPass");
            var token = _httpClient.PostAsync($"{_httpClient.BaseAddress}/Token", contentWithHeaders).Result.
                EnsureSuccessStatusCode().
                Content.ReadAsStringAsync().Result;

            Metadata headers = new();
            headers.Add("Authorization", $"Bearer {token}");

            var actual = _cashboxClient.GetBalances(new Empty(), headers);

            Assert.NotEmpty(actual.Balances);
        }
    }
}