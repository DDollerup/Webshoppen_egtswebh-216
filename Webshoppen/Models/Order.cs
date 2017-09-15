using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.Models
{
    public class Order
    {

        public int ID { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Postal { get; set; }
        public string City { get; set; }

        public string Fullname_Delivery { get; set; }
        public string Phone_Delivery { get; set; }
        public string Email_Delivery { get; set; }
        public string Address_Delivery { get; set; }
        public string Postal_Delivery { get; set; }
        public string City_Delivery { get; set; }

        public List<int> ProductIDs = new List<int>();
    }
}