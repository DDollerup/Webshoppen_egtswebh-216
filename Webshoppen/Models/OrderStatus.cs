using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class OrderStatus
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string Date { get; set; }
        public int StatusID { get; set; }
    }
}