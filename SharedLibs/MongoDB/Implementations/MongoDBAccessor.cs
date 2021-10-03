using MongoDB.Bson;
using MongoDB.Driver;

using SharedLibs.MongoDB.Interfaces;

using System;
using System.Collections.Generic;

namespace SharedLibs.MongoDB.Implementations
{
    public class MongoDBAccessor<T> : IMongoDB<T>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<T> _mongoCollection;

        public MongoDBAccessor(string dbName, string collectionName)
        {
            MongoClient mongoClient = new();

            _mongoDatabase = mongoClient.GetDatabase(dbName);
            _mongoCollection = _mongoDatabase.GetCollection<T>(collectionName);
        }

        public void InsertDTO(T DTO)
        {
            _mongoCollection.InsertOne(DTO);
        }

        public IEnumerable<T> ReadWithFilter(FilterDefinition<T> filterDefinition)
        {
            return _mongoCollection.Find(filterDefinition).ToEnumerable();
        }

        public void UpdateInformation(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition)
        {
            _mongoCollection.UpdateOne(filterDefinition, updateDefinition);
        }

        public void DeleteDocument(FilterDefinition<T> filterDefinition)
        {
            _mongoCollection.DeleteOne(filterDefinition);
        }
    }
}