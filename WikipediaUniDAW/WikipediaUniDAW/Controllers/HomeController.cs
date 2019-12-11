using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WikipediaUniDAW.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return RedirectToRoute("Articles of category", new { categoryId = 1, sortingCriteria = 1 });
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}