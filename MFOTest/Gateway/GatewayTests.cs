using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using MongoDB.Bson;
using MongoDB.Driver;

using Newtonsoft.Json;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using Xunit;

namespace MFOTests
{
    //TODO: по сути, эти тесты являются интеграционными т.к. тестируя gateway мы тестируем и другие микросервисы
    public class GatewayTests
    {
        private readonly HttpClient _httpClient = new();

        public GatewayTests()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44317/api/MFO");
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

        //TODO: when this test launches two times in a row with the same data (updatedDebt), it falls
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

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}/{objectId}"))
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

        [Fact]
        public void GetinformationByDebtIdMustReturnExpectedObject()
        {
            var expected = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").Find(x => x.Id == "61c8b11846629713beada50c").First();

            var reply = _httpClient.GetAsync($"{_httpClient.BaseAddress}/{expected.Id}").Result;
            var actual = reply.Content.ReadFromJsonAsync<Debt>().Result;

            Assert.Equal(expected.Penalty, actual.Penalty);
        }
    }
}