using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using MongoDB.Bson;
using MongoDB.Driver;

using Newtonsoft.Json;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

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
        public void UpdateCreditInformationMustUpdateObjectInDBCorrectly()
        {
            Debt updatedDebt = new Debt() 
            { 
                Id = "61a4fdf9c1c5a7d42fa0df8f",
                Passport = "1234 123456",
                Loan = 1200,
                Issued = DateTime.Parse("2021-01-10T21:00:00.000+00:00"),
                OverdueInDays = 1,
                Penalty = 100,
                Interest = 3
            };

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"{_httpClient.BaseAddress}/{updatedDebt.Id}"))
            {
                var serializedDebt = JsonConvert.SerializeObject(updatedDebt);
                request.Content = new StringContent(serializedDebt);
                _httpClient.Send(request).EnsureSuccessStatusCode();
            }
            
            var debtFromDB = MongoDBAccessor<Debt>.GetMongoCollection("MFO", "Debts").Find(x => x.Id == updatedDebt.Id).First();

            Assert.Equal(updatedDebt.Penalty, debtFromDB.Penalty);
        }

        //TODO: check this test
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