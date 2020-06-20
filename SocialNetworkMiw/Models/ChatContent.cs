using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class ChatContent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Message")]
        public string Message { get; set; }
        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreateTo { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReadTo { get; set; }

        public ChatContent()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

    }
}
