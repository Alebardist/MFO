using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SharedLib.DTO
{
    public class JobProperties
    {
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MonthSalary { get; set; }
    }
}
