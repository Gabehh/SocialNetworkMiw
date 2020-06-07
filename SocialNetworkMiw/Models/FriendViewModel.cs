using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class FriendViewModel
    {
        public string name { get; set; }
        public List<User> friends { get; set; } 
    }
}
