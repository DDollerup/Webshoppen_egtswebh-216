using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public List<Image> Images { get; set; }
        public Category Category { get; set; }

        public string GetFirstImage()
        {
            if (Images.Count > 0)
            {
                return Images[0].ImageURL;
            }
            return "no-image.png";
        }
    }
}