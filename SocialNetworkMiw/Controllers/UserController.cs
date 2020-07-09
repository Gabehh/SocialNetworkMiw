using System;
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
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService userService;

        public UserController(ILogger<UserController> logger, UserService userService)
        {
            _logger = logger;
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            try
            {
                UserViewModel userViewModel = new UserViewModel()
                {
                    CurrentUserRequests = userService.Get(HttpContext.Session.GetString("UserId")).FriendRequests,
                    Users = userService.GetByName(id)
                };
                return View(userViewModel);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}