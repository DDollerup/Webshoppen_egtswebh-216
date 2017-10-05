using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Webshoppen.Factories;
using Webshoppen.Models;

namespace Webshoppen.Areas.Admin.Controllers
{
    [Authorize]
    public class CMSController : Controller
    {
        ProductFactory productFactory = new ProductFactory();
        ImageFactory imageFactory = new ImageFactory();
        CategoryFactory categoryFactory = new CategoryFactory();
        OrderFactory orderFactory = new OrderFactory();
        OrderCollectionFactory orderCollectionFactory = new OrderCollectionFactory();
        StatusFactory statusFactory = new StatusFactory();
        OrderStatusFactory orderStatusFactory = new OrderStatusFactory();

        #region Login
        [AllowAnonymous]
        public ActionResult Login(string returnurl)
        {
            Session["ReturnURL"] = returnurl;
            return View();
        }

        [AllowAnonymous, ValidateAntiForgeryToken, HttpPost]
        public ActionResult LoginSubmit(string username, string password)
        {
            if (username == "abe" && password == "123")
            {
                FormsAuthentication.SetAuthCookie(username, false);

                string returnURL = Session["ReturnURL"]?.ToString();
                if (returnURL == null)
                {
                    returnURL = "/Admin/CMS/Index";
                }

                return Redirect(returnURL);
            }
            TempData["MSG"] = "Wrong username or password.";
            return RedirectToAction("Login");
        }
        
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion

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

        public ActionResult EditProduct(int id = 0)
        {
            if (id == 0) return RedirectToAction("Products");
            ViewBag.Categories = categoryFactory.GetAll();

            List<Image> images = new List<Image>();
            images.AddRange(imageFactory.GetBy("ProductID", 0));
            images.AddRange(imageFactory.GetBy("ProductID", id));
            ViewBag.Images = images;

            Product p = productFactory.Get(id);

            return View(p);
        }

        [HttpPost]
        public ActionResult EditProductSubmit(Product p, List<int> imageIDs)
        {
            productFactory.Update(p);

            if (imageIDs?.Count > 0)
            {
                for (int i = 0; i < imageIDs.Count; i++)
                {
                    Image img = imageFactory.Get(imageIDs[i]);
                    img.ProductID = p.ID;
                    imageFactory.Update(img);
                }
            }

            foreach (Image img in imageFactory.GetBy("ProductID", p.ID))
            {
                if (imageIDs.Contains(img.ID))
                {
                    continue;
                }
                img.ProductID = 0;
                imageFactory.Update(img);
            }

            TempData["MSG"] = "The product: " + p.Name + " has been updated.";
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(int id)
        {
            productFactory.Delete(id);

            foreach (Image img in imageFactory.GetBy("ProductID", id))
            {
                img.ProductID = 0;
                imageFactory.Update(img);
            }

            TempData["MSG"] = "A product with ID: " + id + " has been deleted.";
            return RedirectToAction("Products");
        }
        #endregion

        #region Images
        public ActionResult Images()
        {
            List<ImageVM> images = new List<ImageVM>();
            foreach (Image img in imageFactory.GetAll())
            {
                ImageVM imv = new ImageVM();
                imv.Image = img;

                if (img.ProductID != 0)
                {
                    imv.Product = productFactory.Get(img.ProductID);
                }

                if (img.CategoryID != 0)
                {
                    imv.Category = categoryFactory.Get(img.CategoryID);
                }

                images.Add(imv);
            }

            return View(images);
        }

        [HttpPost]
        public ActionResult UploadImagesSubmit(List<HttpPostedFileBase> images)
        {
            foreach (HttpPostedFileBase image in images)
            {
                string fileName = "";
                if (Upload.Image(image, Request.PhysicalApplicationPath + @"/Content/Images/", out fileName))
                {
                    Upload.Image(image, Request.PhysicalApplicationPath + @"/Content/Images/", "tn_"+ fileName, 400);

                    Image imgToDatabase = new Image();
                    imgToDatabase.ImageURL = fileName;
                    imgToDatabase.CategoryID = 0;
                    imgToDatabase.ProductID = 0;
                    imgToDatabase.SubpageID = 0;

                    imageFactory.Insert(imgToDatabase);

                    TempData["MSG"] = "Image(s) has been uploaded.";

                }
            }
            return Redirect(Request.UrlReferrer.PathAndQuery);
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