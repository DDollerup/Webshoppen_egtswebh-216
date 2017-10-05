using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshoppen.Models;
using Webshoppen.Factories;
using System.Security.Cryptography;
using System.Text;

namespace Webshoppen.Controllers
{
    public class UserController : Controller
    {
        UserFactory userFactory = new UserFactory();

        [HttpPost]
        public ActionResult Login(User user)
        {
            SHA512 password = new SHA512Managed();
            password.ComputeHash(Encoding.ASCII.GetBytes(user.Password));

            string hashedPassword = BitConverter.ToString(password.Hash).Replace("-", "").ToLower();

            User userToLogin = userFactory.SqlQuery("SELECT * FROM [User] WHERE Email='" + user.Email + "' AND Password='" + hashedPassword + "'");

            if (userToLogin?.ID > 0)
            {
                Session["User"] = userToLogin;
            }
            else
            {
                TempData["LoginError"] = "Wrong password or email.";
            }


            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult RegisterUser()
        {
            Session["ReturnURL_RegisterUser"] = Request.UrlReferrer.PathAndQuery;
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUserSubmit(User user)
        {
            SHA512 password = new SHA512Managed();
            password.ComputeHash(Encoding.ASCII.GetBytes(user.Password));

            string hashedPassword = BitConverter.ToString(password.Hash).Replace("-", "").ToLower();

            user.Password = hashedPassword;

            userFactory.Insert(user);

            Session["User"] = user;

            string returnURL = Session["ReturnURL_RegisterUser"].ToString();
            Session.Remove("ReturlURL_RegisterUser");

            if (returnURL.ToLower().Contains("registeruser"))
            {
                return Redirect("/Home/Index");
            }

            return Redirect(returnURL);
        }
    }
}