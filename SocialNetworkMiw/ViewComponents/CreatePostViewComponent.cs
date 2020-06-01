using Microsoft.AspNetCore.Mvc;
using SocialNetworkMiw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Views.ViewComponents
{
    public class CreatePostViewComponent: ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            CreatePostViewModel createPostViewModel = new CreatePostViewModel();
            return View(createPostViewModel);
        }
    }
}
