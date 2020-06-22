using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class PorfileDetailsViewModel
    {
        public TypePorfile.Porfile Porfile { get; set; }

        public string Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        [Display(Name = "BirthDay")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Job")]
        public string Job { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "From")]
        public string BornIn { get; set; }

        public List<ShowPostViewModel> Posts { get; set; }
        
        public List<User> Friends { get; set; }

        public List<FriendRequest> FriendRequests { get; set; }

        public List<FriendRequest> CurrentUserRequests { get; set; }
    }

    public class EditPorfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public IFormFile ImageUrl { get; set; }

        [Display(Name = "BirthDate")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Job")]
        public string Job { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "From")]
        public string From { get; set; }

    }
}
