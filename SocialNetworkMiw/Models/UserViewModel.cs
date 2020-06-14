using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class UserViewModel
    {
        public List<User> Users { get; set; } 
        public List<FriendRequest> CurrentUserRequests { get; set; }
    }
}
