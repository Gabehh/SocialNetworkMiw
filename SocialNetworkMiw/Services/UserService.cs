using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> users;

        public UserService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            IMongoDatabase database = client.GetDatabase("SocialNetworkMIW");
            users = database.GetCollection<User>("Users");
        }

        public List<User> Get()
        {
            return users.Find(user => true).ToList();
        }

        public List<User> Get(List<string> ids)
        {
            return users.Find(user => ids.Contains(user.Id)).ToList();
        }

        public User Get(string id)
        {
            return users.Find(user => user.Id == id).FirstOrDefault();
        }

        public User GetByEmail(string email)
        {
            return users.Find(user => user.Email == email).FirstOrDefault();
        }

        public List<User> GetByName(string name)
        {
            return users.Find(user => user.Name.ToLower().Contains(name.ToLower())).ToList();
        }

        public User GetByEmailAndPassword(string email, string password)
        {
            return users.Find(user => user.Email == email && user.Password == password).FirstOrDefault();
        }

        public User Create(User user)
        {
            users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn)
        {
            users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(string id)
        {
            users.DeleteOne(user => user.Id == id);
        }
    }
}
