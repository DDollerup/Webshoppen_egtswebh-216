using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshoppen.Models;
using Webshoppen.Factories;
using Webshoppen.ShoppingCart;

namespace Webshoppen.Controllers
{
    public class HomeController : Controller
    {
        ProductFactory productFactory = new ProductFactory();
        ImageFactory imageFactory = new ImageFactory();
        CategoryFactory categoryFactory = new CategoryFactory();

        // ShoppingCart
        ProductCart cart;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            cart = new ProductCart(HttpContext, "ShoppingCart");
            ViewBag.AmountInCart = cart.GetShoppingCart().Count;
            ViewBag.ShoppingCartTotal = cart.Total;
            base.OnActionExecuting(filterContext);
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            List<CategoryVM> categories = new List<CategoryVM>();

            foreach (Category item in categoryFactory.GetAll())
            {
                CategoryVM categoryVM = new CategoryVM();
                categoryVM.Category = item;
                categoryVM.Image = imageFactory.SqlQuery("SELECT TOP 1 * FROM Image WHERE CategoryID = '" + item.ID + "'");
                categories.Add(categoryVM);
            }

            return View(categories);
        }

        public ActionResult PartialShowCategories()
        {
            List<CategoryVM> categories = new List<CategoryVM>();

            foreach (Category item in categoryFactory.GetAll())
            {
                CategoryVM categoryVM = new CategoryVM();
                categoryVM.Category = item;
                categoryVM.Image = imageFactory.SqlQuery("SELECT TOP 1 * FROM Image WHERE CategoryID = '" + item.ID + "'");
                categories.Add(categoryVM);
            }
            return PartialView(categories);
        }

        // ID = CategoryID
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

        public ActionResult ShowProduct(int id = 0)
        {
            if (id > 0)
            {
                ProductVM vm = new ProductVM();
                vm.Product = productFactory.Get(id);
                vm.Images = imageFactory.GetBy("ProductID", id);
                vm.Category = categoryFactory.Get(vm.Product.CategoryID);
                return View(vm); 
            }
            else
            {
                return RedirectToAction("Products");
            }
        }

        [HttpPost]
        public ActionResult SearchSubmit(string searchInput)
        {
            TempData["SearchResult"] = productFactory.Search(searchInput, "Name", "Description");
            return RedirectToAction("Products");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int amount)
        {
            ProductVM vm = new ProductVM();
            vm.Product = productFactory.Get(id);
            vm.Images = imageFactory.GetBy("ProductID", id);
            vm.Category = categoryFactory.Get(vm.Product.CategoryID);

            for (int i = 0; i < amount; i++)
            {
                cart.Add(vm);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RemoveFromCart(int id)
        {
            ProductVM vm = cart.GetShoppingCart().Find(x => x.Product.ID == id);
            cart.Remove(vm);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Cart()
        {
            return View(cart.GetShoppingCart());
        }

        public ActionResult CartButton()
        {
            ViewBag.AmountInCart = cart.GetShoppingCart().Count;
            return PartialView();
        }
    }
}