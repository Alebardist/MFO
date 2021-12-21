using System;
using System.IO;
using System.Linq;
using System.Net.Http;

using MongoDB.Bson;

using Newtonsoft.Json;

using SharedLib.DTO;

using Xunit;

namespace MFOTest
{
    public class GatewayTests
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public GatewayTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/MFO/");
        }

        [Fact]
        public void GETResultMustContainExpectedData()
        {
            var expected = File.ReadAllText(@"..\..\..\CreditConveyor\jsonResults\GET_ExpectedResult.json");
            expected = expected.Replace(Environment.NewLine, "");
            expected = expected.Replace(" ", "");
            var body = new StringContent(File.ReadAllText(@"..\..\..\CreditConveyor\jsonBody/GET_body.json"));
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var requestResult = _httpClient.PostAsync(_httpClient.BaseAddress, body).Result;
            var actual = requestResult.Content.ReadAsStringAsync().Result;

            //TODO: check this test
            //requestResult.EnsureSuccessStatusCode();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetCreditHistoryByPassportMustReturnObjectOfCreditHistoryWithExpectedId()
        {
            string result;

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/CreditHistory"))
            {
                request.Headers.Add("PassportNumbers", "1234 123456");
                //TODO: how to get JSON body from reply?
                result = _httpClient.Send(request).Content.ReadAsStringAsync().Result;
            }
            Client creditHistory = JsonConvert.DeserializeObject<Client>(result);

            Assert.True(creditHistory.CreditHistory.First().Id == new ObjectId("61bc8b4b5864f85c6b9eb540"));
        }

    }
}
