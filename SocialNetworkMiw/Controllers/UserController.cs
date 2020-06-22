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

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<User> collectionUser;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            try
            {
                UserViewModel userViewModel = new UserViewModel()
                {
                    CurrentUserRequests = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single().FriendRequests,
                    Users = collectionUser.Find(new BsonDocument("$where", "this.Name == '" + id + "'")).ToList()
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