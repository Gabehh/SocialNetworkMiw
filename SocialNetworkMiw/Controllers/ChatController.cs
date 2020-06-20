using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<User> collectionUser;
        private readonly IMongoCollection<Chat> collectionChat;

        public ChatController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            collectionChat = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Chat>("Chats");
        }

        // GET: Chat
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single();
            if (user == null || !user.Friends.Contains(HttpContext.Session.GetString("UserId")))
                return NotFound();

            var group = collectionChat.Find(u=> u.Friends.Contains(id) && u.Friends.Contains(HttpContext.Session.GetString("UserId")) &&
                                            u.Friends.Count == 2).Single();
            group.Content.ForEach(u =>
            {
                if(u.CreateTo == id && u.ReadTo != HttpContext.Session.GetString("UserId"))
                {
                    u.ReadTo = HttpContext.Session.GetString("UserId");
                }
            });
            collectionChat.ReplaceOne(u => u.Id == group.Id, group);
            ChatViewModel chat = new ChatViewModel()
            {
                Group = group,
                User = user
            };

            return View(chat);
        }
    }

}