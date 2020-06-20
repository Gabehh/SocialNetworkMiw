using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using MongoDB.Bson;
using System;
using System.Linq;

namespace SignalRChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<Chat> collectionChat;

        public ChatHub(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionChat = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Chat>("Chats");
        }


        public Task SendMessage(string message)
        {
            var httpContext = Context.GetHttpContext();
            var group = httpContext.Request.Query["group"].ToString();
            string name = Context.User.Identity.Name;
            var chat = collectionChat.Find(new BsonDocument("$where", "this._id == '" + group + "'")).Single();
            ChatContent chatContent = new ChatContent()
            {
                DateTime = DateTime.Now,
                Message = message,
                CreateTo = Context.User.Claims.ToList()[1].Value
            };
            chat.Content.Add(chatContent);
            collectionChat.ReplaceOneAsync(u => u.Id == chat.Id, chat);
            return Clients.Group(group).SendAsync("ReceiveMessage", name, message, chatContent.Id);
        }


        public void ReadMessage(string id)
        {
            var httpContext = Context.GetHttpContext();
            var group = httpContext.Request.Query["group"].ToString();
            var chat = collectionChat.Find(new BsonDocument("$where", "this._id == '" + group + "'")).Single();
            var content = chat.Content.Find(u => u.Id == id);
            if (content.CreateTo != Context.User.Claims.ToList()[1].Value)
            {
                content.ReadTo = Context.User.Claims.ToList()[1].Value;
                collectionChat.ReplaceOneAsync(u => u.Id == chat.Id, chat);
            }
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var group = httpContext.Request.Query["group"].ToString();
            Groups.AddToGroupAsync(Context.ConnectionId, group);
            return base.OnConnectedAsync();
        }
    }
}