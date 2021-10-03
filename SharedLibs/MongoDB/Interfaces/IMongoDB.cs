using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace SharedLibs.MongoDB.Interfaces
{
    public interface IMongoDB<T>
    {
        /// <summary>
        /// Inserts one DTO
        /// </summary>
        /// <param name="DTO"></param>
        public void InsertDTO(T DTO);

        /// <summary>
        /// Extracts documents according to filter
        /// </summary>
        /// <param name="bsonFilter"></param>
        /// <returns>Enumerable object with all found documents</returns>
        public IEnumerable<T> ReadWithFilter(FilterDefinition<T> filterDefinition);

        /// <summary>
        /// Updates all fields in document with passed Auto.Id
        /// </summary>
        /// <param name="DTO"></param>
        public void UpdateInformation(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition);

        /// <summary>
        /// Deletes document according to passed ObjectId
        /// </summary>
        /// <param name="bsonFilter"></param>
        public void DeleteDocument(FilterDefinition<T> filterDefinition);
    }
}
