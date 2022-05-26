using System;
using System.Net.Http;

using Xunit;

namespace MFOTest.Gateway
{
    public class BCHControllerTests
    {
        private readonly HttpClient _httpClient = new();

        public BCHControllerTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/BCH");
        }

        [Fact]
        public void GetRatingMustReturnCorrectValue()
        {
            string result;

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Rating"))
            {
                request.Headers.Add("PassportNumbers", "1234 123456");
                result = _httpClient.Send(request).EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result;
            }

            Assert.Contains("25", result);
        }

        [Fact]
        public void GetCreditHistoryByPassportMustReturnObjectOfCreditHistoryWithExpectedId()
        {
            string result;

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/CreditHistory"))
            {
                request.Headers.Add("PassportNumbers", "1234 123456");
                result = _httpClient.Send(request).Content.ReadAsStringAsync().Result;
            }

            Assert.Contains("61bb03975864f85c6b9eb53f", result);
        }
    }
}
