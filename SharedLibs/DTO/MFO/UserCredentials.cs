using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class UserCredentials
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string UserName { get; set; }
        [BsonRepresentation(BsonType.Binary)]
        public byte[] UserPassword { get; set; }
    }
}
