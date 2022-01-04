using System;
using System.Net.Http;

using Xunit;

namespace MFOTest.Gateway
{
    public class CashboxController
    {
        private readonly HttpClient _httpClient = new();

        public CashboxController()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/Cashbox");
        }

        [Fact]
        public void GetTokenMustReturnNotEmptyToken()
        {
            var contentWithHeaders = new StringContent("");
            contentWithHeaders.Headers.Add("UserName", "admin");
            contentWithHeaders.Headers.Add("Password", "adminPass");

            var reply = _httpClient.PostAsync($"{_httpClient.BaseAddress}/Token", contentWithHeaders).Result;
            reply.EnsureSuccessStatusCode();

            Assert.NotEmpty(reply.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public void GetBalancesMustReturnExpectedValues()
        {
            var contentWithHeaders = new StringContent("");
            contentWithHeaders.Headers.Add("UserName", "admin");
            contentWithHeaders.Headers.Add("Password", "adminPass");
            var token = _httpClient.PostAsync($"{_httpClient.BaseAddress}/Token", contentWithHeaders).Result.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Balances");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var requestResult = _httpClient.Send(request);

            requestResult.EnsureSuccessStatusCode();

            var jsonResult = requestResult.Content.ReadAsStringAsync().Result;

            Assert.False(string.IsNullOrEmpty(jsonResult));
        }
    }
}