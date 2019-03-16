using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineGunluk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Ana Sayfa";

            return View();
        }


        public ActionResult Login()
        {
            ViewBag.Title = "Giriş Yap";
            return View();
        }   
    }
}
