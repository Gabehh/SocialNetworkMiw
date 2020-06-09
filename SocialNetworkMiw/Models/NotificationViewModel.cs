using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class NotificationViewModel
    {
        public string FriendRequestId { get; set; }
        public string UserId { get; set; }
        [Display(Name = "User")]
        public string UserName { get; set; }
        public string UrlImgUser { get; set; }
        [Display(Name = "Date")]
        public DateTime DateTime { get; set; }
    }
}
