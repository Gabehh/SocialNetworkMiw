using Microsoft.AspNetCore.Mvc;
using SocialNetworkMiw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Views.ViewComponents
{
    public class ShowPostsViewComponent: ViewComponent
    {

        public IViewComponentResult Invoke(List<Post> posts)
        {
            return View(posts);
        }
    }
}
