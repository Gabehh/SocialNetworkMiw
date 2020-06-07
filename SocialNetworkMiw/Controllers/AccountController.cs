using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace SocialNetworkMiw.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoClient mongoClient;

        public AccountController(IConfiguration configuration)
        {
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }

        // POST: Account/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
                if(!collection.Find(new BsonDocument("$where", "this.Email == '" + register.Email + "'")).Any())
                {
                    User user = new User()
                    {
                        Name = register.Name,
                        Email = register.Email,
                        Password =  register.Password,
                        Friends = new List<string>(),
                        Posts = new List<string>(),
                        FriendRequests = new List<FriendRequest>()
                    };
                    collection.InsertOne(user);
                    await SignIn(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The email is already used");
            }
            return View(register);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var user = collection.Find(new BsonDocument("$where", "this.Email == '" + model.Email + "' && this.Password =='" + model.Password+"'")).FirstOrDefault();

            if (user != null)
            {
                await SignIn(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid session");
                return View(model);
            }
        }

        private async Task SignIn(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                });
                HttpContext.Session.SetString("UserId", user.Id);
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
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

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
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