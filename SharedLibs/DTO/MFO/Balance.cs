using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class Balance
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string StorageName { get; set; }
        public decimal Money { get; set; }
    }
}
