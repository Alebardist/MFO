using MongoDB.Driver;

namespace SharedLib.MongoDB.Implementations
{
    public static class MongoDBAccessor<T>
    {
        public static IMongoCollection<T> GetMongoCollection(string dbName, string collectionName)
        {
            IMongoCollection<T> collection = null;
            try
            {
                MongoClient client = new("mongodb://localhost:27017/?readPreference=primary&directConnection=true" +
                "&connectTimeoutMS=3000&tls=true&socketTimeoutMS=3000&serverSelectionTimeoutMS=3000");
                IMongoDatabase mongoDatabase = client.GetDatabase(dbName);
                collection = mongoDatabase.GetCollection<T>(collectionName);
            }
            catch (System.Exception ex)
            {
                throw;
            }

            return collection;
        }
    }
}