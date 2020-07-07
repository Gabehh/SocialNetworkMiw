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
using SocialNetworkMiw.Services;

namespace SocialNetworkMiw.Controllers
{
    [Authorize]
    public class PorfileController : Controller
    {
        private readonly ILogger<PorfileController> _logger;
        private readonly UserService userService;
        private readonly PostService postService;

        public PorfileController(ILogger<PorfileController> logger, UserService userService, PostService postService)
        {
            _logger = logger;
            this.postService = postService;
            this.userService = userService;
        }

        // GET: Porfile/Details/5
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = userService.Get(id);

                if (user == null)
                    return NotFound();

                var currentUser = userService.Get(HttpContext.Session.GetString("UserId"));

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
                porfileViewModel.Posts = postService.GetByUserId(id).Select(u => new ShowPostViewModel()
                {
                    UserName = user.Name,
                    Post = u
                }).OrderByDescending(u => u.Post.CreationDate).ToList();
                porfileViewModel.Friends = userService.Get(user.Friends);
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
                    postService.Create(post);
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

                var user = userService.Get(id);

                if(user == null)
                    return NotFound();

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
                if (!(id == user.Id && id == HttpContext.Session.GetString("UserId")))
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    var _user = userService.Get(user.Id);
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
                    userService.Update(_user.Id, _user);
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

                var post = postService.Get(postId);

                if (post == null)
                    return NotFound();

                if (post.UserId == HttpContext.Session.GetString("UserId"))
                {
                    postService.Remove(post.Id);
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

                if (id == HttpContext.Session.GetString("UserId") || userService.Get(HttpContext.Session.GetString("UserId")).Friends.Any(u => u == id))
                {
                    return View( postService.GetByUserId(id));
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