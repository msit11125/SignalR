using Newtonsoft.Json.Linq;
using signalrMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace signalrMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Chat()
        {
            string username = "";

            var claimsIdentity = User.Identity as ClaimsIdentity;

            if (User.Identity.IsAuthenticated)
            {
                username = User.Identity.Name;
            }

            ViewBag.UserName = username;

            return View();
        }


        public ActionResult Login(User value)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, value.UserName));
            claims.Add(new Claim(ClaimTypes.Role, "Users"));

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignIn(identity);


            return RedirectToAction("Chat");
        }


        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");


            return RedirectToAction("Chat");
        }
    }



    
}