using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class ShowPostViewModel
    {
        public Post Post { get; set; }
        public string UserName { get; set; }
    }
    public class CreatePostViewModel
    {
        [Display(Name = "File")]
        [DataType(DataType.Upload)]
        [Required]
        public IFormFile FileUrl { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }
    }

    public class CreateCommentViewModel
    {
        public string IdPost { get; set; }
        public string Comment { get; set; }
    }

    public class DeleteCommentViewModel
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }
    }
}
