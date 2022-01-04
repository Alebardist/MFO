using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class CreditHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateOfTake { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Summ { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Interest { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Overdues { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsPayed { get; set; }
    }
}