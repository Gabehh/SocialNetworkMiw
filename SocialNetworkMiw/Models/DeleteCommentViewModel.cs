using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkMiw.Models
{
    public class DeleteCommentViewModel
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }
    }
}
