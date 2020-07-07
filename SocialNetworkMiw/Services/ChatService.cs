using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Chat> chats;

        public ChatService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            IMongoDatabase database = client.GetDatabase("SocialNetworkMIW");
            chats = database.GetCollection<Chat>("Chats");
        }

        public List<Chat> Get()
        {
            return chats.Find(chat => true).ToList();
        }


        public List<Chat> GetByUserId(string id)
        {
            return chats.Find(chat => chat.Friends.Contains(id)).ToList();
        }

        public Chat GetByUserIds(string firstUser, string secondUser)
        {
            return chats.Find(chat => chat.Friends.Contains(firstUser) && chat.Friends.Contains(secondUser) && chat.Friends.Count == 2).FirstOrDefault();
        }

        public Chat Get(string id)
        {
            return chats.Find(chat => chat.Id == id).FirstOrDefault();
        }

        public Chat Create(Chat chat)
        {
            chats.InsertOne(chat);
            return chat;
        }

        public void Update(string id, Chat chatIn)
        {
            chats.ReplaceOne(chat => chat.Id == id, chatIn);
        }

        public void Remove(Chat chatIn)
        {
            chats.DeleteOne(chat => chat.Id == chatIn.Id);
        }

        public void Remove(string id)
        {
            chats.DeleteOne(chat => chat.Id == id);
        }
    }
}
