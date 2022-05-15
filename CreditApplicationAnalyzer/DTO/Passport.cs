using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using System.ComponentModel.DataAnnotations;

namespace CreditApplicationsAnalyzer.DTO
{
    public class Passport
    {
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string FIO { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public string SeriaAndNumber { get; set; }
    }
}