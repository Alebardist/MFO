using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class CreditHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string PassportNumber { get; set; }
        public int OverduedDebts { get; set; }
        public int[] Loans { get; set; }
    }
}