using SharedLib.DTO;
using SharedLib.MongoDB.Implementations;

using Xunit;

namespace MFOTest.SharedLib
{
    public class SharedLibTests
    {
        [Fact]
        public void GetCollectionMustReturnRequestedCollection()
        {
            var actual = MongoDBAccessor<Client>.GetMongoCollection("BCH", "Clients");

            Assert.NotNull(actual);
        }
    }
}
