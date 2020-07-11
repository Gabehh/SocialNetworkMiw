using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SocialNetworkMiw.Models;
using SocialNetworkMiw.Services;

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService userService;
        private readonly PostService postService;
        private readonly ChatService chatService;
        private readonly HtmlEncoder htmlEncoder;


        public HomeController(ILogger<HomeController> logger, UserService userService, PostService postService, ChatService chatService, HtmlEncoder htmlEncoder)
        {
            _logger = logger;
            this.chatService = chatService;
            this.postService = postService;
            this.userService = userService;
            this.htmlEncoder = htmlEncoder;
        }


        public IActionResult Index()
        {
            try
            {
                var currentUser = userService.Get(HttpContext.Session.GetString("UserId"));
                var users = currentUser.Friends;
                users.Add(HttpContext.Session.GetString("UserId"));
                var dataPosts = (from user in userService.Get()
                                 join post in postService.Get()
                                 on user.Id equals post.UserId
                                 where users.Contains(post.UserId)
                                 orderby post.CreationDate descending
                                 select new ShowPostViewModel
                                 {
                                     Post = post,
                                     UserName = user.Name
                                 }).Take(50).ToList();
                currentUser.Friends.Remove(HttpContext.Session.GetString("UserId"));
                var filterFriend = Builders<User>.Filter.In(u => u.Id, currentUser.Friends);
                var friends = userService.Get(currentUser.Friends);
                var myChats = chatService.GetByUserId(currentUser.Id);
                var fiendsChats = friends.Select(u => new FriendMessages
                {
                    Friend = u,
                    UnreadMessage = myChats.SelectMany(z => z.Content.Where(z => z.CreateTo == u.Id && z.ReadTo != currentUser.Id)).Count()
                }).ToList();
                return View(new HomeViewModel()
                {
                    Friends = fiendsChats,
                    ShowPost = dataPosts
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }
        }


        // POST: Porfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                        Description = htmlEncoder.Encode(createPostViewModel.Description),
                        CreationDate = DateTime.Now
                    };
                    postService.Create(post);
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error", new ErrorViewModel());
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var post = postService.Get(createCommentViewModel.IdPost);
                Comment comment = new Comment()
                {
                    DateTime = DateTime.Now,
                    Description = htmlEncoder.Encode(createCommentViewModel.Comment),
                    UserName = HttpContext.Session.GetString("UserName"),
                    UserId = HttpContext.Session.GetString("UserId"),
                };

                var currentUser = userService.Get(HttpContext.Session.GetString("UserId"));
                if (post.UserId != currentUser.Id && !currentUser.Friends.Contains(post.UserId))
                    throw new Exception("Post wrong");

                if (post.Comments == null)
                    post.Comments = new List<Comment>()
                    {
                        comment
                    };
                else
                    post.Comments.Add(comment);
                postService.Update(post.Id, post);
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
        [ValidateAntiForgeryToken]
        public JsonResult DeleteComment([FromBody] DeleteCommentViewModel deleteCommentViewModel)
        {
            try
            {
                var post = postService.Get(deleteCommentViewModel.PostId);
                Comment comment = post.Comments.Single(u => u.Id == deleteCommentViewModel.CommentId);
                if (post.UserId == deleteCommentViewModel.UserId || deleteCommentViewModel.UserId == comment.UserId)
                {
                    post.Comments.Remove(comment);
                }
                postService.Update(post.Id, post);
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
