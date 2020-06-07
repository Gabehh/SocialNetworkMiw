﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class RequestFriend
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string UserId  { get; set; }

        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }

        public RequestFriend()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
