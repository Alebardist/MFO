using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

using Xunit;

namespace MFOTest.Gateway
{
    //NOTE: по сути, эти тесты являются интеграционными т.к. тестируя gateway мы тестируем и другие микросервисы
    public class MFOControllerTests
    {
        private readonly HttpClient _httpClient = new();

        public MFOControllerTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/MFO");
        }

        //NOTE: when this test launches two times in a row with the same data (updatedDebt), it falls
        [Fact]
        public void UpdateCreditInformationMustUpdateObjectInDBCorrectly()
        {
            string objectId = "61c8b11846629713beada50c";

            Debt updatedDebt = new()
            {
                Id = objectId,
                Passport = "1234 123456",
                Loan = 320,
                Issued = DateTime.Parse("2021-01-10T21:00:00.000+00:00"),
                OverdueInDays = 10,
                Penalty = new Random().Next(),
                Interest = 3
            };

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}/Debt"))
            {
                request.Content = JsonContent.Create(updatedDebt, mediaType: new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                _httpClient.Send(request).EnsureSuccessStatusCode();
            }

            var debtFromDB = MongoDBAccessor<Debt>.
                GetMongoCollection("MFO", "Debts").
                Find(x => x.Id == objectId).
                First();

            Assert.Equal(updatedDebt.Penalty, debtFromDB.Penalty);
        }

        [Fact]
        public void GetinformationByDebtIdMustReturnExpectedObject()
        {
            var expected = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").Find(x => x.Id == "61c8b11846629713beada50c").First();

            var reply = _httpClient.GetAsync($"{_httpClient.BaseAddress}/{expected.Id}").Result;
            var actual = reply.Content.ReadFromJsonAsync<Debt>().Result;

            Assert.Equal(expected.Penalty, actual.Penalty);
        }

        [Fact]
        public void CheckHealthMustReturnCode429()
        {
            HttpStatusCode expected = HttpStatusCode.TooManyRequests;

            HttpStatusCode actual = HttpStatusCode.OK;

            int requestsCount = 100;

            for (int i = 0; i < requestsCount; i++)
            {
                actual = _httpClient.GetAsync("https://localhost:44317/checkHealth").Result.StatusCode;
            }

            Assert.Equal(expected, actual);
        }
    }
}