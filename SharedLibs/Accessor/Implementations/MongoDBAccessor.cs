using MongoDB.Bson;
using MongoDB.Driver;

using System.Collections.Generic;

namespace SharedLib.MongoDB.Implementations
{
    public static class MongoDBAccessor<T>
    {
        public const string connectionstring = "mongodb://localhost:27017/?readPreference=primary&directConnection=true" +
                "&connectTimeoutMS=3000&tls=true&socketTimeoutMS=3000&serverSelectionTimeoutMS=3000";
        private static readonly MongoClient _client = new(connectionstring);

        public static IMongoCollection<T> GetMongoCollection(string dbName, string collectionName)
        {
            CheckConnectionByGettingListOfDBs();

            IMongoCollection<T> collection = null;
            IMongoDatabase mongoDatabase = _client.GetDatabase(dbName);
            collection = mongoDatabase.GetCollection<T>(collectionName);

            return collection;
        }

        public static List<BsonDocument> CheckConnectionByGettingListOfDBs()
        {
            return _client.ListDatabases().ToList();
        }
    }
}