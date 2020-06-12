using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class FriendViewModel
    {
        public string description { get; set; }
        public string userId { get; set; }
        public List<User> friends { get; set; } 

        public FriendViewModel()
        {
            friends = new List<User>();
        }
    }
}
