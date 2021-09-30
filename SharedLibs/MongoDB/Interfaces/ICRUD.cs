using MongoDB.Bson;

using System.Collections.Generic;

namespace SharedLibs.MongoDB.Interfaces
{
    public interface ICRUD<T>
    {
        /// <summary>
        /// Inserts one DTO
        /// </summary>
        /// <param name="DTO"></param>
        public void WriteDTO(T DTO);

        /// <summary>
        /// Extracts documents according to filter
        /// </summary>
        /// <param name="bsonFilter"></param>
        /// <returns>Enumerable object with all found documents</returns>
        public IEnumerable<T> ReadWithFilter(BsonDocument bsonFilter);

        /// <summary>
        /// Updates all fields in document with passed Auto.Id
        /// </summary>
        /// <param name="DTO"></param>
        public void UpdateInformation(byte[] password, T DTO);

        /// <summary>
        /// Deletes document according to passed ObjectId
        /// </summary>
        /// <param name="bsonFilter"></param>
        public void DeleteDocument(byte[] password);

        public T ReadByPassword(byte[] objectId);
    }
}
