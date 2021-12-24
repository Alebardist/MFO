using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Balance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string StorageName { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Money { get; set; }
    }
}
