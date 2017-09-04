using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webshoppen.Models;

namespace Webshoppen.ShoppingCart
{
    public class ProductCart : ShoppingCart<ProductVM>
    {
        public ProductCart(HttpContextBase contextBase, string sessionName) : base(contextBase, sessionName)
        {
            
        }

        public double Total
        {
            get
            {
                double _total = 0;
                foreach (ProductVM vm in GetShoppingCart())
                {
                    Product p = vm.Product;
                    if (p.Price >= p.SalePrice)
                    {
                        _total += p.Price;
                    }
                    else
                    {
                        _total += p.SalePrice;
                    }
                }

                return _total;
            }
        }
    }
}