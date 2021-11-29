using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.DTO
{
    public class UserCredentials
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string UserName { get; set; }
        public byte[] UserPassword { get; set; }
    }
}
