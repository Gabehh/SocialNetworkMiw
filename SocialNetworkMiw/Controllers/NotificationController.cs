using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;

namespace SocialNetworkMiw.Controllers
{
    public class NotificationController : Controller
    {

        private readonly MongoClient mongoClient;

        public NotificationController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }


        // GET: Notification
        public ActionResult Index()
        {
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
            user.FriendRequests.ForEach(u =>
            {
                var _user = collection.Find(new BsonDocument("$where", "this._id == '" + u.UserId + "'")).Single();
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


        public ActionResult Accept(string idFrindRequest)
        {
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            if(user.FriendRequests.Any(u=>u.Id == idFrindRequest))
            {
                FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                var friend = collection.Find(new BsonDocument("$where", "this._id == '" + request.UserId + "'")).Single();
                user.FriendRequests.Remove(request);
                user.Friends.Add(request.UserId);
                friend.Friends.Add(user.Id);
                collection.ReplaceOne(u => u.Id == friend.Id, friend);
                collection.ReplaceOne(u => u.Id == user.Id, user);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
            return RedirectToAction(nameof(Index));
        }


        public ActionResult Delete(string idFrindRequest)
        {
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            if (user.FriendRequests.Any(u => u.Id == idFrindRequest))
            {
                FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                user.FriendRequests.Remove(request);
                collection.ReplaceOne(u => u.Id == user.Id, user);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
            return RedirectToAction(nameof(Index));
        }

    }
}