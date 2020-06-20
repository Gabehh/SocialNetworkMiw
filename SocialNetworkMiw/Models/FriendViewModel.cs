using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class FriendViewModel
    {
        public string Description { get; set; }
        public string UserId { get; set; }
        public List<User> Friends { get; set; } 

        public FriendViewModel()
        {
            Friends = new List<User>();
        }
    }
}
