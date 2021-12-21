using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Debt
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Passport { get; set; }
        public decimal Loan { get; set; }
        public DateTime Issued { get; set; }
        public decimal Interest { get; set; }
        public int OverdueInDays { get; set; }
        public decimal Penalty { get; set; }
    }
}
