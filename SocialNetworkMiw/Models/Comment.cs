using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }

        public Comment()
        {
            Id = ObjectId.GenerateNewId().ToString();      
        }
    }
}
