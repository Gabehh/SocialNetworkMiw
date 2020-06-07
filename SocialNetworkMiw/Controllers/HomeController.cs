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
            var user = collectionUser
                .Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            var filterPost = Builders<Post>.Filter.In(u => u.Id, user.Posts);
            return View(collectionPost.Find(filterPost).ToList());
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
                var collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
                Post post = new Post()
                {
                    FileUrl = "/Images/" + Path.GetFileName(path),
                    Description = createPostViewModel.Description
                };
                await collectionPost.InsertOneAsync(post);
                var currentUser = collectionUser
                                .Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
                currentUser.Posts.Add(post.Id);
                collectionUser.ReplaceOne(x => x.Id == currentUser.Id, currentUser);
            }
            return RedirectToAction(nameof(Index));

        }



    }
}
