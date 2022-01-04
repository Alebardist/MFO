using System;
using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Debt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Passport { get; set; }

        [Required]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, int.MaxValue)]
        public decimal Loan { get; set; }

        [Required]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Issued { get; set; }

        [Required]
        [Range(0, 100)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Interest { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [BsonRepresentation(BsonType.Int32)]
        public int OverdueInDays { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Penalty { get; set; }
    }
}