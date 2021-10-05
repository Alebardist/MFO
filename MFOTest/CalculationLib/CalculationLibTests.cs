using CalculationLib;
using MongoDB.Driver;
using SharedLib.DTO;
using System.Linq;
using Xunit;

namespace MFOTest.CalculationLib
{
    public class CalculationLibTests
    {
        private readonly IMongoCollection<CreditHistory> _mongoCollection = new MongoClient().GetDatabase("BCH")
            .GetCollection<CreditHistory>("Credit_Histories");

        [Fact]
        public void GetCreditRatingMustReturnExpectedValue()
        {
            var creditHistory = _mongoCollection.Find(x => x.Id == new MongoDB.Bson.ObjectId("6159f5c04fb6b04a8fa37180")).First();
            var creditParameters = new CreditParameters()
            {
                MoneyToLoan = 4000
            };
            var actual = new CreditCalculations().GetCreditRating(creditHistory, creditParameters);

            Assert.Equal(100, actual);
        }
    }
}
