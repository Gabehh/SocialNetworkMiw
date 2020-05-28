using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;

namespace SocialNetworkMiw.Controllers
{
    public class PorfileController : Controller
    {
        private readonly MongoClient mongoClient;

        public PorfileController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }

        // GET: Porfile/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + id + "'")).FirstOrDefault();

            if (user == null)
                return NotFound();

            var currentUser = collection
                            .Find(new BsonDocument("$where", "this._id == '" + User.FindFirst(ClaimTypes.NameIdentifier)
                            .Value.ToString() + "'")).Single();

            PorfileViewModel porfileViewModel = new PorfileViewModel();
            if (currentUser.Id == id) porfileViewModel.Porfile = TypePorfile.Porfile.User;
            else if(currentUser.Friends.Contains(new ObjectId(id))) porfileViewModel.Porfile = TypePorfile.Porfile.Friend;
            else porfileViewModel.Porfile = TypePorfile.Porfile.Unknown;

            porfileViewModel.BirthDate = user.BirthDate;
            porfileViewModel.BornIn = user.BornIn;
            porfileViewModel.Email = user.Email;
            porfileViewModel.Job = user.Job;
            //porfileViewModel.Posts
            //porfileViewModel.Photos 
            porfileViewModel.City = user.City;
            porfileViewModel.Id = user.Id;
            porfileViewModel.ImageUrl = user.ImageUrl;
            porfileViewModel.Name = user.Name;
            return View(porfileViewModel);
        }



        // GET: Porfile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Porfile/Create
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

        // GET: Porfile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Porfile/Edit/5
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

        // GET: Porfile/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Porfile/Delete/5
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