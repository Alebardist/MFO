using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.Collections.Generic;

namespace SharedLib.DTO
{
    public class Client
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Passport { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string FIO { get; set; }

        public IEnumerable<CreditHistory> CreditHistory { get; set; }
        public JobProperties JobProperties { get; set; }
    }
}