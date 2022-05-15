using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using System.ComponentModel.DataAnnotations;

namespace CreditApplicationsAnalyzer.DTO
{
    internal class CreditApplication
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        //[Required]
        //[BsonRepresentation(BsonType.Document)]
        public Passport PassportData { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public string ContactPhoneNumber { get; set; }

        [Required]
        [Range(5000, int.MaxValue)]
        [BsonRepresentation(BsonType.Int32)]
        public int WishedAmount { get; set; }

        [Required]
        [Range(30, int.MaxValue)]
        [BsonRepresentation(BsonType.Int32)]
        public int TermInDays { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [BsonRepresentation(BsonType.Int32)]
        public int MonthIncome { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [BsonRepresentation(BsonType.Int32)]
        public int MonthlyCreditServiceSum { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public string Education { get; set; }

        //[Required]
        //[BsonRepresentation(BsonType.Document)]
        public Job JobObject { get; set; }
    }
}