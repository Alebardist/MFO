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
        [BsonRepresentation(BsonType.String)]
        public string Passport { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string FIO { get; set; }
        [BsonRepresentation(BsonType.Array)]
        public IEnumerable<CreditHistory> CreditHistory { get; set; }
        public JobProperties JobProperties { get; set; }
    }
}