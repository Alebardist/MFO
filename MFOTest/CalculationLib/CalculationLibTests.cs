using CalculationLib;

using MongoDB.Driver;

using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using System.Linq;

using Xunit;

namespace MFOTest.CalculationLib
{
    public class CalculationLibTests
    {
        [Fact]
        public void GetCreditRatingMustReturnExpectedValue()
        {
            var expected = 25;
            var creditHistories = MongoDBAccessor<Client>.
                GetMongoCollection("BCH", "Clients").
                Find(x => x.Passport == "1234 123456").First().CreditHistory;

            var actual = CreditCalculations.GetCreditRating(creditHistories);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDebtMustReturnCorrectDebtOnCurrentDate()
        {
            var expected = 2000;
            var creditHistoriy = MongoDBAccessor<Client>.
                GetMongoCollection("BCH", "Clients").
                Find(x => x.Passport == "1234 123456").
                First().CreditHistory;

            decimal actual = creditHistoriy.Where(x => !x.IsPayed).Sum(x => x.Summ);

            Assert.Equal(expected, actual);
        }
    }
}