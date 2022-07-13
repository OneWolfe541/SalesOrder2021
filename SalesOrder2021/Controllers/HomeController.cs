using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesOrder2021.Context;
using SalesOrder2021.Methods;
using SalesOrder2021.Models;

namespace SalesOrder2021.Controllers
{
    public class HomeController : Controller
    {        

        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Search", "Sales");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(User userLog)
        {
            var usr = UserMethods.GetUser(1);
            //var usr = UserMethods.ValidateUser(userLog);
            if (usr != null)
            {
                Session["UserId"] = usr.UserId;
                Session["Username"] = usr.UserName;
                return RedirectToAction("Search", "Sales");
            }
            else
            {
                ModelState.AddModelError("", "Username or Password is incorrect.");
                ViewBag.LogError = "Username or Password is incorrect.";
                return View("Login");
            }
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            Session["Username"] = null;
            //Session["RoleId"] = null;
            Session.Abandon();
            return View("Logout");
        }

    }
}