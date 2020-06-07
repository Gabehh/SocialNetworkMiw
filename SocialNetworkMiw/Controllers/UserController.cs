using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserController : Controller
    {
        private readonly MongoClient mongoClient;

        public UserController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }



        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Details/5
        [HttpGet]
        public ActionResult Details(string id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + userId + "'")).Single();
            ViewData["MyFrienRequests"] = user.FriendRequests;       
            return View(collection.Find(new BsonDocument("$where", "this.Name == '" + id + "'")).ToList());
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
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

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
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

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
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