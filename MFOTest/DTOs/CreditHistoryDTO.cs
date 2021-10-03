using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MFOTest.DTOs
{
    internal class CreditHistoryDTO
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string PassportNumber { get; set; }
        public int OverduedDebts { get; set; }
        public int[] Loans { get; set; }
    }
}