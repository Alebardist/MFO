using System.Collections.Generic;

namespace SharedLib.Accessor.Interfaces
{
    internal interface IBasicDBOperations<T>
    {
        /// <summary>
        /// Inserts one record
        /// </summary>
        /// <param name="DTO"></param>
        public void CreateNewRecord(T DTO);

        /// <summary>
        /// Extracts record according to id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Enumerable object with all found records</returns>
        public IEnumerable<T> GetRecordByID(string id);

        /// <summary>
        /// Updates record with passed Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedDTO"></param>
        public void UpdateRecordByID(string id, T updatedDTO);

        /// <summary>
        /// Deletes record according to passed id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteRecordByID(string id);
    }
}
