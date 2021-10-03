using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;
using System;
using Xunit;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MFOTest.SharedLib
{
    public class SharedLibTests
    {
        private readonly IMongoCollection<CreditHistory> _mongoCollection = new MongoClient().GetDatabase("BCH")
            .GetCollection<CreditHistory>("Credit_Histories");
        private MongoDBAccessor<CreditHistory> _mongoDB = new("BCH", "Credit_Histories");

        [Fact]
        public void ReadWithFilterMustReturnExpectedData()
        {
            var expected = new CreditHistory()
            {
                Id = new ObjectId("615864fff04ef5ef9a003f1f"),
                PassportNumber = "1234 123456",
                OverduedDebts = 0,
                Loans = new int[] { 1, 2, 3 }
            };
            _mongoCollection.InsertOne(expected);

            var operationResult = _mongoDB.ReadWithFilter(new FilterDefinitionBuilder<CreditHistory>().Eq(x => x.Id, new ObjectId("615864fff04ef5ef9a003f1f")));

            var doc = operationResult.GetEnumerator();
            doc.MoveNext();
            Assert.Equal(expected.Id, doc.Current.Id);
        }

        [Fact]
        public void UpdateInformationMustChangeDocumentWithGivenIdToPassedDTO()
        {
            //Arrange
            var dtoToUpdate = new CreditHistory()
            {
                Id = new ObjectId("615864fff04ef5ef9a003f1f"),
                PassportNumber = "1234 123456",
                OverduedDebts = 0,
                Loans = new int[] { 1, 2, 3 }
            };

            _mongoCollection.InsertOne(dtoToUpdate);

            var filter = new FilterDefinitionBuilder<CreditHistory>().Eq(x => x.Id, dtoToUpdate.Id);
            var updateDefinition = new UpdateDefinitionBuilder<CreditHistory>().Set(dtoToUpdate => dtoToUpdate.Loans, new int[] { 1, 2, 3, 4 });

            //Action
            _mongoDB.UpdateInformation(filter, updateDefinition);

            //Assert
            CreditHistory extracted = _mongoCollection.Find(x => x.Id == dtoToUpdate.Id).First();
            Assert.NotEqual(dtoToUpdate.Loans, extracted.Loans);

            //CleanUp
            _mongoCollection.DeleteOne(x => x.Id == dtoToUpdate.Id);
        }
    }
}
