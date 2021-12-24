using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class UserCredentials
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public byte[] UserPassword { get; set; }
    }
}
