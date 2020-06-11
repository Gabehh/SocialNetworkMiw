using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class EditPorfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        [DataType(DataType.Upload)]
        [Required]
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
