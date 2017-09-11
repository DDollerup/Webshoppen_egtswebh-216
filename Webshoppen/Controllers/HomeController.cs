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
            ViewBag.AmountInCart = cart.Amount();
            ViewBag.ShoppingCartTotal = cart.Total;
            base.OnActionExecuting(filterContext);
        }

        #region Main
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
        public ActionResult ContactSubmit(string name, string email, string subject, string message)
        {
            EmailClient emailClient = new EmailClient("smtp.gmail.com", 587, "webitumbraco@gmail.com", "FedeAbe2000", true);
            emailClient.SendNotification(name, email, subject, message);
            TempData["ContactMSG"] = "Din besked er blevet modtaget, se din mail for bekræftelse.";
            return RedirectToAction("Contact");
        }
        #endregion

        #region ShoppingCart
        [HttpPost]
        public ActionResult AddToCart(int id, int amount)
        {

            Product p = productFactory.Get(id);
            cart.Add(p);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RemoveFromCart(int id)
        {
            Product p = productFactory.Get(id);
            cart.Remove(p);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Cart()
        {
            List<List<ProductVM>> shoppingCartList = new List<List<ProductVM>>();

            foreach (List<Product> productLine in cart.GetShoppingCart())
            {
                List<ProductVM> lvm = new List<ProductVM>();
                foreach (Product product in productLine)
                {
                    ProductVM vm = new ProductVM();
                    vm.Product = product;
                    vm.Category = categoryFactory.Get(product.CategoryID);
                    vm.Images = imageFactory.GetBy("ProductID", product.ID);
                    lvm.Add(vm);
                }
                shoppingCartList.Add(lvm);
            }
            return View(shoppingCartList);
        }

        public ActionResult CartButton()
        {
            ViewBag.AmountInCart = cart.Amount();
            return PartialView();
        }
        #endregion

        #region Checkout
        public ActionResult Checkout()
        {
            List<List<ProductVM>> shoppingCartList = new List<List<ProductVM>>();

            foreach (List<Product> productLine in cart.GetShoppingCart())
            {
                List<ProductVM> lvm = new List<ProductVM>();
                foreach (Product product in productLine)
                {
                    ProductVM vm = new ProductVM();
                    vm.Product = product;
                    vm.Category = categoryFactory.Get(product.CategoryID);
                    vm.Images = imageFactory.GetBy("ProductID", product.ID);
                    lvm.Add(vm);
                }
                shoppingCartList.Add(lvm);
            }
            return View(shoppingCartList);
        }

        [HttpPost]
        public ActionResult CheckoutSubmit(Order order)
        {
            EmailClient emailClient = new EmailClient("smtp.gmail.com", 587, "webitumbraco@gmail.com", "FedeAbe2000", true);
            //emailClient.SendEmail(order.Email);
            //emailClient.SendNotification(order.Fullname, order.Email, Request.PhysicalApplicationPath + @"\EmailTemplates\Notification.html");

            List<Product> productsInOrder = new List<Product>();
            for (int i = 0; i < order.ProductIDs.Count; i++)
            {
                productsInOrder.Add(productFactory.Get(order.ProductIDs[i]));
            }

            List<ProductVM> products = new List<ProductVM>();
            foreach (Product item in productsInOrder)
            {
                ProductVM productVM = new ProductVM();
                productVM.Product = item;
                productVM.Images = imageFactory.GetBy("ProductID", item.ID);
                productVM.Category = categoryFactory.Get(item.CategoryID);

                products.Add(productVM);
            }

            emailClient.SendInvoice(order, products);
            return RedirectToAction("OrderConfirmation");
        }

        public ActionResult OrderConfirmation()
        {
            return View();
        }
        #endregion
    }
}