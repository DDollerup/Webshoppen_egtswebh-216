using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.ShoppingCart
{
    public class ShoppingCart<T>
    {
        private List<T> allEntities;

        private HttpContextBase contextBase;
        private string sessionName;

        public ShoppingCart(HttpContextBase contextBase, string sessionName)
        {

            this.contextBase = contextBase;
            this.sessionName = sessionName;

            if (this.contextBase.Session[sessionName] != null)
            {
                allEntities = this.contextBase.Session[sessionName] as List<T>;
            }
            else
            {
                allEntities = new List<T>();
            }
        }

        private void RefreshSession()
        {
            this.contextBase.Session[sessionName] = allEntities;
        }

        public void Add(T entity)
        {
            allEntities.Add(entity);
            RefreshSession();
        }

        public void Remove(T entity)
        {
            allEntities.Remove(entity);
            RefreshSession();
        }

        public void Clear()
        {
            allEntities.Clear();
            RefreshSession();
        }

        public List<T> GetShoppingCart()
        {
            return allEntities;
        }
    }
}