using System;
using System.Collections.Generic;
using System.IO;
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

            var collectionUsers = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            var collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
            var user = collectionUsers.Find(new BsonDocument("$where", "this._id == '" + id + "'")).FirstOrDefault();
            
            if (user == null) 
                return NotFound();

            var currentUser = collectionUsers
                            .Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();

            PorfileViewModel porfileViewModel = new PorfileViewModel();

            if (currentUser.Id == id) 
                porfileViewModel.Porfile = TypePorfile.Porfile.User;
            else if(currentUser.Friends.Contains(id)) 
                porfileViewModel.Porfile = TypePorfile.Porfile.Friend;
            else 
                porfileViewModel.Porfile = TypePorfile.Porfile.Unknown;

            porfileViewModel.BirthDate = user.BirthDate;
            porfileViewModel.BornIn = user.BornIn;
            porfileViewModel.Email = user.Email;
            porfileViewModel.Job = user.Job;
            var filterPost = Builders<Post>.Filter.In(u=>u.Id, user.Posts);
            porfileViewModel.Posts = collectionPost.Find(filterPost).ToList();
            var filterFriend = Builders<User>.Filter.In(u => u.Id, user.Friends);
            porfileViewModel.Friends = collectionUsers.Find(filterFriend).ToList();
            //porfileViewModel.Photos 
            porfileViewModel.City = user.City;
            porfileViewModel.Id = user.Id;
            porfileViewModel.ImageUrl = user.ImageUrl;
            porfileViewModel.Name = user.Name;
            porfileViewModel.FriendRequests = user.FriendRequests;
            ViewData["MyFrienRequests"] = currentUser.FriendRequests;
            return View(porfileViewModel);
        }




        public ActionResult AddFriend(string id, string returnUrl)
        {
            var collection = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<User>("Users");
            FriendRequest requestFriend = new FriendRequest()
            {
                DateTime = DateTime.Now,
                UserId = HttpContext.Session.GetString("UserId"),
            };
            var user = collection.Find(new BsonDocument("$where", "this._id == '" + id + "'")).Single();
            if (user.FriendRequests.Any(u => u.UserId == HttpContext.Session.GetString("UserId")) 
                    || user.Friends.Any(u=>u== HttpContext.Session.GetString("UserId")))
            {
                return View("Error", new ErrorViewModel());
            }
            else
            {
                user.FriendRequests.Add(requestFriend);
                collection.ReplaceOne(x => x.Id == user.Id, user);
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                else
                    return Redirect(returnUrl);
            }
        }


        // GET: Porfile/Create
        public ActionResult Create()
        {
            return View();
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
                collectionPost.InsertOne(post);
                var currentUser = collectionUser
                                .Find(new BsonDocument("$where", "this._id == '" + HttpContext.Session.GetString("UserId") + "'")).Single();
                currentUser.Posts.Add(post.Id);
                collectionUser.ReplaceOne(x => x.Id == currentUser.Id, currentUser);
            }
            return RedirectToAction(nameof(Details), new { id = HttpContext.Session.GetString("UserId")});

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
                var collectionPost = mongoClient.GetDatabase("SocialNetworkMIW").GetCollection<Post>("Posts");
                var post = collectionPost
                           .Find(new BsonDocument("$where", "this._id == '" + createCommentViewModel.IdPost + "'")).Single();
                Comment comment = new Comment()
                {
                    DateTime = DateTime.Now,
                    Description = createCommentViewModel.Comment,
                    User = HttpContext.Session.GetString("UserName") 
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
                    comment
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