using MongoDB.Bson;
using MongoDB.Driver;

using Newtonsoft.Json;

using RabbitMQ.Client;
using SharedLib.DTO.Application;
using SharedLib.MongoDB.Implementations;

using System.Linq;
using System.Text;

using Xunit;

namespace MFOTest.CreditApplicationAnalyzer
{
    public class CreditApplicationAnalyzerServiceTests
    {
        [Fact]
        public void PublishedJSONMessageMustBeWrittenInDB()
        {
            CreditApplication creditApplication = new CreditApplication()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                PassportData = new Passport() { FIO = "123", SeriaAndNumber = "1234" },
                ContactPhoneNumber = "123",
                WishedAmount = 40000,
                TermInDays = 40,
                MonthIncome = 23000,
                MonthlyCreditServiceSum = 1,
                Education = "colledge",
                JobObject = new Job() { CuratorsWorkPhone = "123", LocalAdress = "addr", OrganizationName = "Company" }
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
{
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(creditApplication));

                channel.BasicPublish(exchange: "amq.direct",
                                     routingKey: "CreditApplicationKey",
                                     basicProperties: null,
                                     body: body);
            }

            var collection = MongoDBAccessor<CreditApplication>.GetMongoCollection("CreditApplicationsService",
                                                                "CreditApplications");
            var idFromDB = collection.Find(x => x.Id == creditApplication.Id).First().Id;

            Assert.Equal(creditApplication.Id, idFromDB);

            collection.DeleteOne(x => x.Id == creditApplication.Id);
        }
    }
}
