using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;

namespace SocialNetworkMiw.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("FileUrl")]
        [DataType(DataType.Upload)]
        public string FileUrl { get; set; }

        [BsonElement("Comments")]
        public List<Comment> Comments { get; set; }

        public string Type 
        { 
            get 
            { 
                return GetTypeFile(); 
            } 
        }

        private string GetTypeFile()
        {
            string contentType = string.Empty;
            if (new FileExtensionContentTypeProvider().TryGetContentType(FileUrl, out contentType))
            {
                return contentType.Split("/")[0];
            }
            return string.Empty;
        }

    }
}
