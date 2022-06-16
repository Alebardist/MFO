using MongoDB.Bson;
using MongoDB.Driver;

using System.Collections.Generic;

namespace SharedLib.MongoDB.Implementations
{
    public static class MongoDBAccessor<T>
    {
        public const string connectionstring = "mongodb://localhost:27017/?readPreference=primary&directConnection=true" +
                                                "&serverSelectionTimeoutMS=3000";
        private static readonly MongoClient _client = new(connectionstring);

        public static IMongoCollection<T> GetMongoCollection(string dbName, string collectionName)
        {
            CheckConnectionByGettingListOfDBs();
            IMongoDatabase mongoDatabase = _client.GetDatabase(dbName);
            return mongoDatabase.GetCollection<T>(collectionName);
        }

        public static List<BsonDocument> CheckConnectionByGettingListOfDBs()
        {
            return _client.ListDatabases().ToList();
        }
    }
}