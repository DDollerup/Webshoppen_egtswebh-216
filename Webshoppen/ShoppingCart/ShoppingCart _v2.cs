using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshoppen.ShoppingCart
{
    public class ShoppingCart_v2<T>
    {
        private List<List<T>> allEntities;

        private HttpContextBase contextBase;
        private string sessionName;

        public ShoppingCart_v2(HttpContextBase contextBase, string sessionName)
        {

            this.contextBase = contextBase;
            this.sessionName = sessionName;

            if (this.contextBase.Session[sessionName] != null)
            {
                allEntities = this.contextBase.Session[sessionName] as List<List<T>>;
            }
            else
            {
                allEntities = new List<List<T>>();
            }
        }

        private void RefreshSession()
        {
            this.contextBase.Session[sessionName] = allEntities;
        }

        private object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public void Add(T entity)
        {
            List<T> list = null;
            foreach (List<T> shoppingCartLine in allEntities)
            {
                if (shoppingCartLine.Count > 0 && (int)GetPropValue(shoppingCartLine[0], "ID") == (int)GetPropValue(entity, "ID"))
                {
                    list = shoppingCartLine;
                    break;
                }
            }

            if (list != null)
            {
                list.Add(entity);
            }
            else
            {
                allEntities.Add(new List<T> { entity });
            }

            RefreshSession();
        }

        public void Remove(T entity)
        {
            List<T> list = null;
            foreach (List<T> shoppingCartLine in allEntities)
            {
                if (shoppingCartLine.Count > 0 && (int)GetPropValue(shoppingCartLine[0], "ID") == (int)GetPropValue(entity, "ID"))
                {
                    list = shoppingCartLine;
                    break;
                }
            }

            list.RemoveAt(0);

            if (list.Count <= 0)
            {
                allEntities.Remove(list);
            }

            RefreshSession();
        }

        public void Delete(T entity)
        {
            List<T> list = null;
            foreach (List<T> shoppingCartLine in allEntities)
            {
                if (shoppingCartLine.Count > 0 && (int)GetPropValue(shoppingCartLine[0], "ID") == (int)GetPropValue(entity, "ID"))
                {
                    list = shoppingCartLine;
                    break;
                }
            }

            allEntities.Remove(list);
            RefreshSession();
        }

        public void Clear()
        {
            allEntities.Clear();
            RefreshSession();
        }

        public List<List<T>> GetShoppingCart()
        {
            return allEntities;
        }
    }
}