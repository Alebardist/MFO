using System;
using System.IO;
using System.Net.Http;
using Xunit;

namespace MFOTest
{
    public class CreditControllerTests
    {
        private HttpClient _httpClient = new HttpClient();

        public CreditControllerTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/Credit");
        }

        [Fact]
        public void GETResultMustContainExpectedData()
        {
            var expected = File.ReadAllText(@"..\..\..\CCGW\jsonResults\GET_ExpectedResult.json");
            expected = expected.Replace(Environment.NewLine, "");
            expected = expected.Replace(" ", "");
            var body = new StringContent(File.ReadAllText(@"..\..\..\CCGW\jsonBody/GET_body.json"));
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var requestResult = _httpClient.PostAsync(_httpClient.BaseAddress, body).Result;
            var actual = requestResult.Content.ReadAsStringAsync().Result;

            requestResult.EnsureSuccessStatusCode();
            Assert.Equal(expected, actual);
        }
    }
}
