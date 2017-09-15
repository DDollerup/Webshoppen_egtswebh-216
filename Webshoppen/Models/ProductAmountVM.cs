using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class ProductAmountVM
    {
        public ProductVM ProductVM { get; set; }
        public int Amount { get; set; }

        public double Total()
        {
            return ProductVM.Product.GetPrice() * Amount;
        }
    }
}