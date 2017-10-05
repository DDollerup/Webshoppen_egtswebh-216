using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class CommentVM
    {
        public Comment Comment { get; set; }
        public User User { get; set; }
    }
}