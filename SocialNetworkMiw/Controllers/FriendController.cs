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
                friendViewModel.name = "Your Friends";
            }
            else if(currentUser.Friends.Any(u => u == id))
            {
                var user = collection.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single(); 
                friendViewModel.friends = collection.Find(Builders<User>.Filter.In(u => u.Id, user.Friends)).ToList(); 
                friendViewModel.name = String.Concat(user.Name,"'s"," friends");
            }





            //var currentUser = collection.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            //if (HttpContext.Session.GetString("UserId") == id || currentUser.Friends.Any(u => u == id))
            //{
            //    var user = collection.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single();
            //    var filterFriend = Builders<User>.Filter.In(u => u.Id, user.Friends);
            //    return View(collection.Find(filterFriend).ToList());
            //}
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

        // POST: Friend/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}