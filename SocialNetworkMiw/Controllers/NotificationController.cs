using System.Collections.Generic;
using System.Linq;
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
    public class NotificationController : Controller
    {

        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<User> collectionUser;
        private readonly IMongoCollection<Chat> collectionChat;

        public NotificationController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            collectionChat = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Chat>("Chats");
        }


        // GET: Notification
        public ActionResult Index()
        {
            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
            user.FriendRequests.ForEach(u =>
            {
                var _user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + u.UserId + "'")).Single();
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


        public ActionResult Accept(string idFrindRequest, string returnUrl)
        {
            if (string.IsNullOrEmpty(idFrindRequest))
                return NotFound();

            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            if(user.FriendRequests.Any(u=>u.Id == idFrindRequest))
            {
                FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                var friend = collectionUser.Find(new BsonDocument("$where", "this._id == '" + request.UserId + "'")).Single();
                user.FriendRequests.Remove(request);
                user.Friends.Add(request.UserId);
                friend.Friends.Add(user.Id);
                collectionUser.ReplaceOne(u => u.Id == friend.Id, friend);
                collectionUser.ReplaceOne(u => u.Id == user.Id, user);
                Chat chat = new Chat()
                {
                    Friends = new List<string>()
                    {
                        user.Id,
                        friend.Id
                    },
                    Content = new List<ChatContent>()
                };
                collectionChat.InsertOne(chat);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
            return Redirect(returnUrl);
        }

        public ActionResult Delete(string idFrindRequest)
        {
            if (string.IsNullOrEmpty(idFrindRequest))
                return NotFound();

            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            if (user.FriendRequests.Any(u => u.Id == idFrindRequest))
            {
                FriendRequest request = user.FriendRequests.Single(u => u.Id == idFrindRequest);
                user.FriendRequests.Remove(request);
                collectionUser.ReplaceOne(u => u.Id == user.Id, user);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
            return RedirectToAction(nameof(Index));
        }

    }
}