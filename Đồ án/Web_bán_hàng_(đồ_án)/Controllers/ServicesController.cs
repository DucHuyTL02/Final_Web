using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Services
        public ActionResult Warranty()
        {
            return View();
        }

        public ActionResult Repair()
        {
            return View();
        }

        public ActionResult OtherServices()
        {
            return View();
        }
    }
}