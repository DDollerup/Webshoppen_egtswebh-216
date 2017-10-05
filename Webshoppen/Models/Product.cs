using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webshoppen.Models
{
    public class Product
    {
        public Product()
        {
            CategoryID = 0;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string DateAdded { get; set; }
        public double Price { get; set; }
        public double SalePrice { get; set; }
        public bool Active { get; set; }

        public double GetPrice()
        {
            if (SalePrice < Price)
            {
                return SalePrice;
            }
            return Price;
        }
    }
}