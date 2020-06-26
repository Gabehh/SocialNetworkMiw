using System;
using System.Collections.Generic;
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
    public class PorfileController : Controller
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<User> collectionUser;
        private readonly IMongoCollection<Post> collectionPost;
        private readonly ILogger<PorfileController> _logger;

        public PorfileController(ILogger<PorfileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            mongoClient = new MongoClient(configuration.GetConnectionString("SocialNetwork"));
            collectionUser = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
        }

        // GET: Porfile/Details/5
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = collectionUser
                            .Find(new BsonDocument("$where", "this._id == '" + id + "'")).FirstOrDefault();

                if (user == null)
                    return NotFound();

                var currentUser = collectionUser
                                .Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();

                PorfileDetailsViewModel porfileViewModel = new PorfileDetailsViewModel();

                if (currentUser.Id == id)
                    porfileViewModel.Porfile = TypePorfile.Porfile.User;
                else if (currentUser.Friends.Contains(id))
                    porfileViewModel.Porfile = TypePorfile.Porfile.Friend;
                else
                    porfileViewModel.Porfile = TypePorfile.Porfile.Unknown;

                porfileViewModel.BirthDate = user.BirthDate;
                porfileViewModel.BornIn = user.BornIn;
                porfileViewModel.Email = user.Email;
                porfileViewModel.Job = user.Job;
                porfileViewModel.Posts = collectionPost.Find(new BsonDocument("$where", "this.UserId == '" + id + "'")).ToList().Select(u => new ShowPostViewModel()
                {
                    UserName = user.Name,
                    Post = u
                }).OrderByDescending(u => u.Post.CreationDate).ToList();
                var filterFriend = Builders<User>.Filter.In(u => u.Id, user.Friends);
                porfileViewModel.Friends = collectionUser.Find(filterFriend).ToList();
                porfileViewModel.City = user.City;
                porfileViewModel.Id = user.Id;
                porfileViewModel.ImageUrl = user.ImageUrl;
                porfileViewModel.Name = user.Name;
                porfileViewModel.FriendRequests = user.FriendRequests;
                porfileViewModel.CurrentUserRequests = currentUser.FriendRequests;
                return View(porfileViewModel);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel createPostViewModel)
        {
            try
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
                return RedirectToAction(nameof(Details), new { id = HttpContext.Session.GetString("UserId") });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                if (id != HttpContext.Session.GetString("UserId"))
                {
                    return View("Error", new ErrorViewModel());
                }

                var user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single();

                return View(new EditPorfileViewModel()
                {
                    BirthDate = user.BirthDate,
                    City = user.City,
                    From = user.BornIn,
                    Id = user.Id,
                    Job = user.Job,
                    Name = user.Name
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditPorfileViewModel user)
        {
            try
            {
                if (id != user.Id && id != HttpContext.Session.GetString("UserId"))
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    var _user = collectionUser.Find(new BsonDocument("$where", "this._id == '" + user.Id + "'")).Single();
                    _user.BirthDate = user.BirthDate;
                    _user.BornIn = user.From;
                    _user.City = user.City;
                    _user.Job = user.Job;
                    _user.Name = user.Name;
                    if (user.ImageUrl != null)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.ImageUrl.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await user.ImageUrl.CopyToAsync(stream);
                        }
                        _user.ImageUrl = "/Images/" + Path.GetFileName(path);
                    }
                    collectionUser.ReplaceOne(u => u.Id == _user.Id, _user);
                    HttpContext.Session.SetString("UserName", _user.Name);
                    HttpContext.Session.SetString("UserImage", _user.ImageUrl);
                    return RedirectToAction(nameof(Details), new { id = _user.Id });
                }
                else
                {
                    return View(user);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }

        public ActionResult DeletePost(string postId, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(postId))
                    return NotFound();

                var post = collectionPost.Find(new BsonDocument("$where", "this._id == '" + postId + "'")).FirstOrDefault();

                if (post == null)
                    return NotFound();

                if (post.UserId == HttpContext.Session.GetString("UserId"))
                {
                    collectionPost.DeleteOne(u => u.Id == post.Id);
                    return Redirect(returnUrl);
                }
                else
                {
                    return View("Error", new ErrorViewModel());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }


        public ActionResult Photos(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                if (id == HttpContext.Session.GetString("UserId") ||
                    collectionUser.Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single().Friends.Any(u => u == id))
                {
                    return View(collectionPost.Find(new BsonDocument("$where", "this.UserId == '" + id + "'")).ToList());
                }
                else
                {
                    return View("Error", new ErrorViewModel());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}