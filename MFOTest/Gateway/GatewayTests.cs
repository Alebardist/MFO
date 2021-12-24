using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using MongoDB.Bson;

using Newtonsoft.Json;

using SharedLib.DTO;

using Xunit;

namespace MFOTest
{
    //TODO: по сути, эти тесты являются интеграционными т.к. тестируя gateway мы тестируем и другие микросервисы
    public class GatewayTests
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public GatewayTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/MFO");
        }

        [Fact]
        public void GetCreditHistoryByPassportMustReturnObjectOfCreditHistoryWithExpectedId()
        {
            string result;

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/CreditHistory"))
            {
                request.Headers.Add("PassportNumbers", "1234 123456");
                result = _httpClient.Send(request).EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result;
            }

            Assert.Contains("61bb03975864f85c6b9eb53f", result);
        }
    }
}