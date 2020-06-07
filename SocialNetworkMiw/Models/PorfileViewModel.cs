using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class PorfileViewModel
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

        public string BornIn { get; set; }

        public List<Post> Posts { get; set; }
        
        public List<User> Friends { get; set; }

        public List<RequestFriend> RequestFriends { get; set; }
        
    }
}
