using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class OrderStatusVM
    {
        public OrderStatus OrderStatus { get; set; }
        public Status Status { get; set; }
    }
}