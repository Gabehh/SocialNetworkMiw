using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using SocialNetworkMiw.Services;

namespace SignalRChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly HtmlEncoder htmlEncoder;
        private readonly ChatService chatService;

        public ChatHub(HtmlEncoder htmlEncoder, ChatService chatService)
        {
            this.htmlEncoder = htmlEncoder;
            this.chatService = chatService;
        }


        public Task SendMessage(string message)
        {
            var httpContext = Context.GetHttpContext();
            var group = httpContext.Request.Query["group"].ToString();
            string name = Context.User.Identity.Name;
            var chat = chatService.Get(group);
            ChatContent chatContent = new ChatContent()
            {
                DateTime = DateTime.Now,
                Message = htmlEncoder.Encode(message),
                CreateTo = Context.User.Claims.ToList()[1].Value
            };
            chat.Content.Add(chatContent);
            chatService.Update(chat.Id, chat);
            return Clients.Group(group).SendAsync("ReceiveMessage", name, htmlEncoder.Encode(message), chatContent.Id);
        }


        public void ReadMessage(string id)
        {
            var httpContext = Context.GetHttpContext();
            var group = httpContext.Request.Query["group"].ToString();
            var chat = chatService.Get(group);
            var content = chat.Content.Find(u => u.Id == id);
            if (content.CreateTo != Context.User.Claims.ToList()[1].Value)
            {
                content.ReadTo = Context.User.Claims.ToList()[1].Value;
                chatService.Update(chat.Id, chat);
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