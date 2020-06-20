using System;
using System.Collections.Generic;
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
        private readonly IMongoCollection<User> collectionUser;
        private readonly IMongoCollection<Post> collectionPost;
        private readonly IMongoCollection<Chat> collectionChat;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
            collectionChat = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Chat>("Chats");
        }


        public IActionResult Index()
        {
            var currentUser = collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
            var users = currentUser.Friends;
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
            currentUser.Friends.Remove(HttpContext.Session.GetString("UserId"));
            var filterFriend = Builders<User>.Filter.In(u => u.Id, currentUser.Friends);
            var friends = collectionUser.Find(filterFriend).ToList();
            var myChats = collectionChat.Find(y => y.Friends.Contains(currentUser.Id)).ToList();
            var fiendsChats = friends.Select(u => new FriendMessages
            {
                Friend = u,
                UnreadMessage = myChats.SelectMany(z=>z.Content.Where(z=>z.CreateTo == u.Id && z.ReadTo != currentUser.Id)).Count()
            }).ToList();
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Friends = fiendsChats,
                ShowPost = data.ToList()
            };
            return View(homeViewModel);
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

        [HttpPost]
        public JsonResult WriteComment([FromBody] CreateCommentViewModel createCommentViewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(createCommentViewModel.Comment))
                {
                    return Json(new
                    {
                        isValid = false
                    });
                }
                var post = collectionPost
                           .Find(new BsonDocument("$where", "this._id == '" + createCommentViewModel.IdPost + "'")).Single();
                Comment comment = new Comment()
                {
                    DateTime = DateTime.Now,
                    Description = createCommentViewModel.Comment,
                    UserName = HttpContext.Session.GetString("UserName"),
                    UserId = HttpContext.Session.GetString("UserId"),
                };
                if (post.Comments == null)
                    post.Comments = new List<Comment>()
                    {
                        comment
                    };
                else
                    post.Comments.Add(comment);
                collectionPost.ReplaceOne(x => x.Id == post.Id, post);
                return Json(new
                {
                    isValid = true,
                    comment,
                    postId = post.Id
                });
            }
            catch
            {
                return Json(new
                {
                    isValid = false
                });
            }
        }




        [HttpPost]
        public JsonResult DeleteComment([FromBody] DeleteCommentViewModel deleteCommentViewModel)
        {
            try
            {
                var post = collectionPost
                           .Find(new BsonDocument("$where", "this._id == '" + deleteCommentViewModel.PostId + "'")).Single();
                Comment comment = post.Comments.Single(u => u.Id == deleteCommentViewModel.CommentId);
                if (post.UserId == deleteCommentViewModel.UserId || deleteCommentViewModel.UserId == comment.UserId)
                {
                    post.Comments.Remove(comment);
                }
                collectionPost.ReplaceOne(x => x.Id == post.Id, post);
                return Json(new
                {
                    isValid = true,
                });
            }
            catch
            {
                return Json(new
                {
                    isValid = false
                });
            }
        }


    }
}
