using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class HomeViewModel
    {
        public List<ShowPostViewModel> ShowPost { get; set; }
        public List<FriendMessages> Friends { get; set; }
    }

    public class FriendMessages
    {
        public User Friend { get; set; }
        public int UnreadMessage { get; set; }
    }
}
