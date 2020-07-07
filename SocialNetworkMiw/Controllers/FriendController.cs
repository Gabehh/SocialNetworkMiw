using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
    public class FriendController : Controller
    {

        private readonly ILogger<FriendController> _logger;
        private readonly UserService userService;
        private readonly ChatService chatService;

        public FriendController(ILogger<FriendController> logger, UserService userService, ChatService chatService)
        {
            _logger = logger;
            this.userService = userService;
            this.chatService = chatService;
        }

        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                FriendViewModel friendViewModel = new FriendViewModel();
                var currentUser = userService.Get(HttpContext.Session.GetString("UserId"));

                if (id == currentUser.Id)
                {
                    friendViewModel.Friends = userService.Get(currentUser.Friends);
                    friendViewModel.Description = "Your Friends";
                }
                else if (currentUser.Friends.Any(u => u == id))
                {
                    var user = userService.Get(id);
                    friendViewModel.Friends = userService.Get(user.Friends);
                    friendViewModel.Description = String.Concat(user.Name, "'s", " friends");
                }
                else
                {
                    friendViewModel.Description = "You can't see the friends list";
                }
                friendViewModel.UserId = id;
                return View(friendViewModel);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        public ActionResult AddFriend(string id, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = userService.Get(id);

                if (user == null)
                    return NotFound();

                FriendRequest requestFriend = new FriendRequest()
                {
                    DateTime = DateTime.Now,
                    UserId = HttpContext.Session.GetString("UserId"),
                };

                if (user.FriendRequests.Any(u => u.UserId == HttpContext.Session.GetString("UserId"))
                        || user.Friends.Any(u => u == HttpContext.Session.GetString("UserId")))
                {
                    return View("Error", new ErrorViewModel());
                }
                else
                {
                    user.FriendRequests.Add(requestFriend);
                    userService.Update(user.Id, user);
                    return Redirect(returnUrl);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }


        public ActionResult DeleteFriend(string friendId, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(friendId))
                    return NotFound();

                var friend = userService.Get(friendId);

                if (friend == null)
                    return NotFound();

                var user = userService.Get(HttpContext.Session.GetString("UserId"));


                if (user.Friends.Any(u => u == friend.Id) && friend.Friends.Any(u => u == user.Id))
                {
                    user.Friends.Remove(friendId);
                    friend.Friends.Remove(user.Id);
                    userService.Update(user.Id, user);
                    userService.Update(friend.Id, friend);
                    var groupId = chatService.GetByUserIds(friend.Id, user.Id).Id;
                    chatService.Remove(groupId);
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
    }
}