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

        public FriendController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }

        // GET: Friend
        public ActionResult Index()
        {
            return View();
        }

        // GET: Friend/Details/5
        public ActionResult Details(string id)
        {
            FriendViewModel friendViewModel = new FriendViewModel();
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var currentUser = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            if (id == currentUser.Id)
            {            
                friendViewModel.friends = collection.Find(Builders<User>.Filter.In(u => u.Id, currentUser.Friends)).ToList();
                friendViewModel.description = "Your Friends";
            }
            else if(currentUser.Friends.Any(u => u == id))
            {
                var user = collection.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single(); 
                friendViewModel.friends = collection.Find(Builders<User>.Filter.In(u => u.Id, user.Friends)).ToList(); 
                friendViewModel.description = String.Concat(user.Name,"'s"," friends");
            }
            else
            {
                friendViewModel.description = "You can't see the friends list";
            }
            friendViewModel.userId = id;
            return View(friendViewModel);
        }

        // POST: Friend/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Friend/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Friend/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Friend/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        public ActionResult DeleteFriend(string friendId, string returnUrl)
        {
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            var friend = collection.Find(new BsonDocument("$where", "this._id == '" + friendId + "'")).Single();
            if (user.Friends.Any(u => u == friend.Id) && friend.Friends.Any(u => u == user.Id))
            {
                user.Friends.Remove(friendId);
                friend.Friends.Remove(user.Id);
                collection.ReplaceOne(u => u.Id == user.Id, user);
                collection.ReplaceOne(u => u.Id == friend.Id, friend);
                return Redirect(returnUrl);
            }
            else
            {
                return View("Error", new ErrorViewModel());
            }
        }
    }
}