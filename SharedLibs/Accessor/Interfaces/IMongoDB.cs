using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SharedLib.MongoDB.Interfaces
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
        public IEnumerable<T> ReadWithFilter(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Updates all fields in document with passed Auto.Id
        /// </summary>
        /// <param name="DTO"></param>
        public void UpdateInformation(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition);

        /// <summary>
        /// Deletes document according to passed ObjectId
        /// </summary>
        /// <param name="bsonFilter"></param>
        public void DeleteDocument(Expression<Func<T, bool>> filter);
    }
}