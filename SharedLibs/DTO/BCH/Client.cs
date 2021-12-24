using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Passport { get; set; }
        public string FIO { get; set; }
        public IEnumerable<CreditHistory> CreditHistory { get; set; }
        public JobProperties JobProperties { get; set; }
    }
}