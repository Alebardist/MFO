using MongoDB.Driver;

namespace SharedLib.MongoDB.Implementations
{
    public static class MongoDBAccessor<T>
    {
        public static IMongoCollection<T> GetMongoCollection(string dbName, string collectionName)
        {
            MongoClient client = new();
            IMongoDatabase mongoDatabase = client.GetDatabase(dbName);

            return mongoDatabase.GetCollection<T>(collectionName);
        }
    }
}