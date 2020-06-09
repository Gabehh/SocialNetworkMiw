using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class HomeController : Controller
    {
        private readonly MongoClient mongoClient;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
        }


        public IActionResult Index()
        {
            var collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
            var collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var users = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single().Friends;
            users.Add(HttpContext.Session.GetString("UserId"));
            var data = (from user in collectionUser.AsQueryable() join post in collectionPost.AsQueryable()
                       on user.Id equals post.UserId
                       where users.Contains(post.UserId)
                       orderby post.CreationDate descending
                       select new ShowPostViewModel
                       {
                          Post = post,
                          UserName = user.Name
                       }).Take(50);
            return View(data.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // POST: Porfile/Create
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel createPostViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", createPostViewModel.FileUrl.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await createPostViewModel.FileUrl.CopyToAsync(stream);
                }
                var collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
                Post post = new Post()
                {
                    UserId = HttpContext.Session.GetString("UserId"),
                    FileUrl = "/Images/" + Path.GetFileName(path),
                    Description = createPostViewModel.Description,
                    CreationDate = DateTime.Now
                };
                await collectionPost.InsertOneAsync(post);
            }
            return RedirectToAction(nameof(Index));

        }



    }
}
