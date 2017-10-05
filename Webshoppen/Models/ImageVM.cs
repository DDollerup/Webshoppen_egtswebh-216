using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class ImageVM
    {
        public Image Image { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}