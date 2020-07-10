using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NotificationController : Controller
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly UserService userService;
        private readonly ChatService chatService;

        public NotificationController(ILogger<NotificationController> logger, UserService userService, ChatService chatService)
        {
            _logger = logger;
            this.userService = userService;
            this.chatService = chatService;
        }


        // GET: Notification
        public ActionResult Index()
        {
            try
            {
                var user = userService.Get(HttpContext.Session.GetString("UserId"));
                List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
                user.FriendRequests.ForEach(u =>
                {
                    var _user = userService.Get(u.UserId);
                    notificationViewModels.Add(new NotificationViewModel()
                    {
                        DateTime = u.DateTime,
                        UserId = _user.Id,
                        FriendRequestId = u.Id,
                        UserName = _user.Name,
                        UrlImgUser = _user.ImageUrl,
                    });
                });

                return View(notificationViewModels);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept(string idFrindRequest, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(idFrindRequest))
                    return NotFound();

                var user = userService.Get(HttpContext.Session.GetString("UserId"));
                if (user.FriendRequests.Any(u => u.Id == idFrindRequest))
                {
                    FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                    var friend = userService.Get(request.UserId);
                    user.FriendRequests.Remove(request);
                    user.Friends.Add(request.UserId);
                    friend.Friends.Add(user.Id);
                    userService.Update(friend.Id, friend);
                    userService.Update(user.Id, user);
                    Chat chat = new Chat()
                    {
                        Friends = new List<string>()
                    {
                        user.Id,
                        friend.Id
                    },
                        Content = new List<ChatContent>()
                    };
                    chatService.Create(chat);
                }
                else
                {
                    return View("Error", new ErrorViewModel());
                }
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return View("Error", new ErrorViewModel());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string idFrindRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(idFrindRequest))
                    return NotFound();

                var user = userService.Get(HttpContext.Session.GetString("UserId"));
                if (user.FriendRequests.Any(u => u.Id == idFrindRequest))
                {
                    FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                    user.FriendRequests.Remove(request);
                    userService.Update(user.Id, user);
                }
                else
                {
                    return View("Error", new ErrorViewModel());
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

    }
}