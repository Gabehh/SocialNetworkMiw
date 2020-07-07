using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Post> posts;

        public PostService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            IMongoDatabase database = client.GetDatabase("SocialNetworkMIW");
            posts = database.GetCollection<Post>("Posts");
        }

        public List<Post> Get()
        {
            return posts.Find(post => true).ToList();
        }


        public List<Post> GetByUserId(string userId)
        {
            return posts.Find(post => post.UserId == userId).ToList();
        }


        public Post Get(string id)
        {
            return posts.Find(post => post.Id == id).FirstOrDefault();
        }

        public Post Create(Post post)
        {
            posts.InsertOne(post);
            return post;
        }

        public void Update(string id, Post postIn)
        {
            posts.ReplaceOne(post => post.Id == id, postIn);
        }

        public void Remove(Post postIn)
        {
            posts.DeleteOne(post => post.Id == postIn.Id);
        }

        public void Remove(string id)
        {
            posts.DeleteOne(post => post.Id == id);
        }
    }
}
