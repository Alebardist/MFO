using MongoDB.Bson;
using MongoDB.Driver;

using SharedLib.MongoDB.Interfaces;

using System;
using System.Collections.Generic;

namespace SharedLib.MongoDB.Implementations
{
    public class CrudOperations<T> : ICRUD<T>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<T> _mongoCollection;

        public CrudOperations(string dbName, string collectionName)
        {
            MongoClient mongoClient = new();

            _mongoDatabase = mongoClient.GetDatabase(dbName);
            _mongoCollection = _mongoDatabase.GetCollection<T>(collectionName);
        }

        public void WriteDTO(T DTO)
        {
            _mongoCollection.InsertOne(DTO);
        }

        public IEnumerable<T> ReadWithFilter(BsonDocument bsonFilter)
        {
            return _mongoCollection.Find(bsonFilter).ToList();
        }

        /// <summary>
        /// Extracts document with given ObjectId
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Found DTO of type T</returns>
        /// <throws>If no documents found throws exception</throws>
        public T ReadByPassword(byte[] password)
        {
            return _mongoCollection.Find($"{{PasswordHash:'{password}'}}").First();
        }

        public void UpdateInformation(byte[] password, T updatedDTO)
        {
            var filter = Builders<T>.Filter.Eq("PasswordHash", password);
            _mongoCollection.DeleteOne(filter);
            _mongoCollection.InsertOne(updatedDTO);
        }

        public void DeleteDocument(byte[] password)
        {
            var filter = Builders<T>.Filter.Eq("PasswordHash", password);
            _mongoCollection.DeleteOne(filter);
        }
    }
}