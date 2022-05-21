using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.DTO.Application
{
    public class Job
    {
        [Required]
        [BsonRepresentation(BsonType.String)]
        public string OrganizationName { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public string LocalAdress { get; set; }

        [Required]
        [BsonRepresentation(BsonType.String)]
        public string CuratorsWorkPhone { get; set; }
    }
}