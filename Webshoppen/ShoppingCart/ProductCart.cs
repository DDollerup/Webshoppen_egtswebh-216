using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webshoppen.Models;

namespace Webshoppen.ShoppingCart
{
    public class ProductCart : ShoppingCart_v2<Product>
    {
        public ProductCart(HttpContextBase contextBase, string sessionName) : base(contextBase, sessionName)
        {
            
        }

        public int Amount()
        {
            int count = 0;

            foreach (List<Product> listItem in GetShoppingCart())
            {
                count += listItem.Count;
            }

            return count;
        }

        public double Total
        {
            get
            {
                double _total = 0;
                foreach (List<Product> listItem in GetShoppingCart())
                {
                    foreach (Product p in listItem)
                    {  
                        if (p.Price >= p.SalePrice)
                        {
                            _total += p.Price;
                        }
                        else
                        {
                            _total += p.SalePrice;
                        }
                    }
                }

                return _total;
            }
        }
    }
}