using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("ImageUrl")]
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [BsonElement("BirthDay")]
        public DateTime BirthDate { get; set; }

        [BsonElement("Job")]
        public string Job { get; set; }

        [BsonElement("City")]
        public string City { get; set; }

        [BsonElement("BornIn")]
        public string BornIn { get; set; }

        [BsonElement("Posts")]
        public List<string> Posts { get; set; }

        [BsonElement("Friends")]
        public List<string> Friends { get; set; }
    }
}
