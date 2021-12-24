using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Balance
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string StorageName { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Money { get; set; }
    }
}
