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
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<User> collectionUser;
        private readonly IMongoCollection<Chat> collectionChat;
        public FriendController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            collectionChat = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Chat>("Chats");
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            FriendViewModel friendViewModel = new FriendViewModel();
            var currentUser = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).FirstOrDefault();

            if (id == currentUser.Id)
            {            
                friendViewModel.Friends = collectionUser.Find(Builders<User>.Filter.In(u => u.Id, currentUser.Friends)).ToList();
                friendViewModel.Description = "Your Friends";
            }
            else if(currentUser.Friends.Any(u => u == id))
            {
                var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single(); 
                friendViewModel.Friends = collectionUser.Find(Builders<User>.Filter.In(u => u.Id, user.Friends)).ToList(); 
                friendViewModel.Description = String.Concat(user.Name,"'s"," friends");
            }
            else
            {
                friendViewModel.Description = "You can't see the friends list";
            }
            friendViewModel.UserId = id;
            return View(friendViewModel);
        }

        public ActionResult AddFriend(string id, string returnUrl)
        {

            if(string.IsNullOrEmpty(id))
                return NotFound();

            FriendRequest requestFriend = new FriendRequest()
            {
                DateTime = DateTime.Now,
                UserId = HttpContext.Session.GetString("UserId"),
            };

            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + id + "'")).FirstOrDefault();

            if (user == null)
                return NotFound();

            if (user.FriendRequests.Any(u => u.UserId == HttpContext.Session.GetString("UserId"))
                    || user.Friends.Any(u => u == HttpContext.Session.GetString("UserId")))
            {
                return View("Error", new ErrorViewModel());
            }
            else
            {
                user.FriendRequests.Add(requestFriend);
                collectionUser.ReplaceOne(x => x.Id == user.Id, user);
                return Redirect(returnUrl);
            }
        }


        public ActionResult DeleteFriend(string friendId, string returnUrl)
        {
            if (string.IsNullOrEmpty(friendId))
                return NotFound();

            var friend = collectionUser.Find(new BsonDocument("$where", "this._id == '" + friendId + "'")).FirstOrDefault();

            if (friend == null)
                return NotFound();

            var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();


            if (user.Friends.Any(u => u == friend.Id) && friend.Friends.Any(u => u == user.Id))
            {
                user.Friends.Remove(friendId);
                friend.Friends.Remove(user.Id);
                collectionUser.ReplaceOne(u => u.Id == user.Id, user);
                collectionUser.ReplaceOne(u => u.Id == friend.Id, friend);
                var groupId = collectionChat.Find(u => u.Friends.Contains(friend.Id)
                            && u.Friends.Contains(user.Id) && u.Friends.Count == 2).Single().Id;
                collectionChat.DeleteMany(u => u.Id == groupId);

                return Redirect(returnUrl);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
        }
    }
}