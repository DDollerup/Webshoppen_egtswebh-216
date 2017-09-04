using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class Image
    {
        public int ID { get; set; }
        public string ImageURL { get; set; }
        public int ProductID { get; set; }
        public int SubpageID { get; set; }
        public int CategoryID { get; set; }
    }
}