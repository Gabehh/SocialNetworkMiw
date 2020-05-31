using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
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
}
