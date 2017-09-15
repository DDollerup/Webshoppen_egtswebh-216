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
        ProductFactory productFactory = new ProductFactory();
        ImageFactory imageFactory = new ImageFactory();
        CategoryFactory categoryFactory = new CategoryFactory();
        OrderFactory orderFactory = new OrderFactory();
        OrderCollectionFactory orderCollectionFactory = new OrderCollectionFactory();
        StatusFactory statusFactory = new StatusFactory();
        OrderStatusFactory orderStatusFactory = new OrderStatusFactory();

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

            if (imageIDs != null)
            {
                for (int i = 0; i < imageIDs.Count; i++)
                {
                    Image img = imageFactory.Get(imageIDs[i]);
                    img.ProductID = productID;
                    imageFactory.Update(img);
                } 
            }

            TempData["MSG"] = "A product has been added.";

            return RedirectToAction("Products");
        }
        #endregion

        #region Orders
        public ActionResult Orders()
        {
            List<OrderStatusVM> orders = new List<OrderStatusVM>();

            foreach (OrderStatus orderStatus in orderStatusFactory.GetAll())
            {
                OrderStatusVM vm = new OrderStatusVM();
                vm.OrderStatus = orderStatus;
                vm.Status = statusFactory.Get(orderStatus.StatusID);

                orders.Add(vm);
            }

            return View(orders);
        }

        public ActionResult ShowOrder(int id = 0)
        {
            if (id == 0) return RedirectToAction("Orders");

            List<ProductAmountVM> products = new List<ProductAmountVM>();
            foreach (OrderCollection orderCollection in orderCollectionFactory.GetBy("OrderID", id))
            {
                ProductAmountVM pavm = new ProductAmountVM();

                ProductVM productVM = new ProductVM();
                productVM.Product = productFactory.Get(orderCollection.ProductID);
                productVM.Images = imageFactory.GetBy("ProductID", productVM.Product.ID);
                productVM.Category = categoryFactory.Get(productVM.Product.CategoryID);

                pavm.ProductVM = productVM;
                pavm.Amount = orderCollection.Amount;

                products.Add(pavm);
            }

            OrderVM ovm = new OrderVM();
            ovm.Order = orderFactory.Get(id);
            ovm.Products = products;

            return View(ovm);

        }
        #endregion
    }
}