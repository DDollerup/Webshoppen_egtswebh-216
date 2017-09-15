using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public List<ProductAmountVM> Products { get; set; }
    }
}