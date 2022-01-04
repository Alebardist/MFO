using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class CreditParameters
    {
        [BsonRepresentation(BsonType.Int32)]
        public int MoneyToLoan { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TermDays { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int InterestRate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string PassportNumber { get; set; }
    }
}