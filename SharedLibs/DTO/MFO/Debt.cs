using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Debt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string Passport { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Loan { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Issued { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Interest { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        public int OverdueInDays { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Penalty { get; set; }
    }
}
