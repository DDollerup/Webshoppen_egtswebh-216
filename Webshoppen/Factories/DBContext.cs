using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webshoppen.Models;

namespace Webshoppen
{
    public class DBContext
    {
        private AutoFactory<Product> _productFactory;
        private AutoFactory<Category> _categoryFactory;
        private AutoFactory<Image> _imageFactory;

        public AutoFactory<Product> ProductFactory
        {
            get
            {
                if (_productFactory == null)
                {
                    _productFactory = new AutoFactory<Product>();
                }
                return _productFactory;
            }
        }

        public AutoFactory<Category> CategoryFactory
        {
            get
            {
                if (_categoryFactory == null)
                {
                    _categoryFactory = new AutoFactory<Category>();
                }
                return _categoryFactory;
            }
        }

        public AutoFactory<Image> ImageFactory
        {
            get
            {
                if (_imageFactory == null)
                {
                    _imageFactory = new AutoFactory<Image>();
                }
                return _imageFactory;
            }
        }
    }
}