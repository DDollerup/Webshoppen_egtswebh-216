using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }

    }
}