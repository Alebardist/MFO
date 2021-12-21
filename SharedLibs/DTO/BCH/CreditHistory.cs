using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class CreditHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime DateOfTake { get; set; }
        public decimal Summ { get; set; }
        public decimal Interest { get; set; }
        public int Overdues { get; set; }
        public bool IsPayed { get; set; }
    }
}