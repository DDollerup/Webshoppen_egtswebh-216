using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshoppen.Factories;
using Webshoppen.Models;

namespace Webshoppen.Areas.Admin.Controllers
{
    public class CMSController : Controller
    {
        private ProductFactory productFactory = new ProductFactory();
        private CategoryFactory categoryFactory = new CategoryFactory();
        private ImageFactory imageFactory = new ImageFactory();

        // GET: Admin/CMS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Products(int? id)
        {
            List<ProductVM> products = new List<ProductVM>();

            if (TempData["SearchResult"] != null)
            {
                foreach (Product item in TempData["SearchResult"] as List<Product>)
                {
                    ProductVM productVM = new ProductVM();
                    productVM.Product = item;
                    productVM.Images = imageFactory.GetBy("ProductID", item.ID);
                    productVM.Category = categoryFactory.Get(item.CategoryID);

                    products.Add(productVM);
                }
            }
            else if (id == null)
            {
                foreach (Product item in productFactory.GetAll())
                {
                    ProductVM productVM = new ProductVM();
                    productVM.Product = item;
                    productVM.Images = imageFactory.GetBy("ProductID", item.ID);
                    productVM.Category = categoryFactory.Get(item.CategoryID);

                    products.Add(productVM);
                }
            }
            else
            {
                foreach (Product item in productFactory.GetBy("CategoryID", id))
                {
                    ProductVM productVM = new ProductVM();
                    productVM.Product = item;
                    productVM.Images = imageFactory.GetBy("ProductID", item.ID);
                    productVM.Category = categoryFactory.Get(item.CategoryID);

                    products.Add(productVM);
                }
            }

            return View(products);
        }

        [HttpPost]
        public ActionResult SearchSubmit(string searchInput)
        {
            List<Product> p = productFactory.Search(searchInput, "Name", "Description");
            TempData["SearchResult"] = productFactory.Search(searchInput, "Name", "Description");
            return RedirectToAction("Products");
        }

        #region Products
        public ActionResult AddProduct()
        {
            ViewBag.Categories = categoryFactory.GetAll();
            ViewBag.Images = imageFactory.GetBy("ProductID", 0);
            return View();
        }
        
        
        [HttpPost]
        public ActionResult AddProductSubmit(Product product, List<int> imageIDs)
        {
            int productID = productFactory.Insert(product);

            for (int i = 0; i < imageIDs.Count; i++)
            {
                Image img = imageFactory.Get(imageIDs[i]);
                img.ProductID = productID;
                imageFactory.Update(img);
            }

            TempData["MSG"] = "A product has been added.";

            return RedirectToAction("Products");
        }
        #endregion
    }
}