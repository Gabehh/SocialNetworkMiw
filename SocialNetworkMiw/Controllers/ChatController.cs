using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using SocialNetworkMiw.Services;

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly UserService userService;
        private readonly ChatService chatService;

        public ChatController(ILogger<ChatController> logger, UserService userService, ChatService chatService)
        {
            _logger = logger;
            this.userService = userService;
            this.chatService = chatService;
        }

        public ActionResult Index(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = userService.Get(id);
                if (user == null || !user.Friends.Contains(HttpContext.Session.GetString("UserId")))
                    return NotFound();

                var group = chatService.GetByUserIds(id, HttpContext.Session.GetString("UserId"));
                group.Content.ForEach(u =>
                {
                    if (u.CreateTo == id && u.ReadTo != HttpContext.Session.GetString("UserId"))
                    {
                        u.ReadTo = HttpContext.Session.GetString("UserId");
                    }
                });
                chatService.Update(group.Id, group);
                return View (new ChatViewModel()
                {
                    Group = group,
                    User = user
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }
    }

}