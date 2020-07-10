using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using SocialNetworkMiw.Services;

namespace SocialNetworkMiw.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService userService;
        private readonly HtmlEncoder htmlEncoder;

        public AccountController(UserService userService, HtmlEncoder htmlEncoder)
        {
            this.userService = userService;
            this.htmlEncoder = htmlEncoder;
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                if(userService.GetByEmail(register.Email) == null)
                {
                    string password = string.Empty;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        password = GetMd5Hash(md5Hash, register.Password);
                    }
                    User user = new User()
                    {
                        Name = htmlEncoder.Encode(register.Name),
                        Email = htmlEncoder.Encode(register.Email),
                        Password = password,
                        Friends = new List<string>(),
                        FriendRequests = new List<FriendRequest>(),
                        ImageUrl = "/Images/icons/face.png"
                    };
                    userService.Create(user);
                    await SignIn(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The email is already used");
            }
            return View(register);
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string password = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                password = GetMd5Hash(md5Hash, model.Password);
            }

            var user = userService.GetByEmailAndPassword(model.Email, password);

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
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserImage", user.ImageUrl);
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