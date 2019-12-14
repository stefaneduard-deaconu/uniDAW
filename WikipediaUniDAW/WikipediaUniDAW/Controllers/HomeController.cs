using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers {
    public class HomeController : Controller {

        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index(int sort = 0, bool desc = false) {
            IOrderedQueryable<Article> articles = null;
            ViewBag.Message = "Homepage";

            if (articles == null)
                articles = from article in db.Articles
                           orderby article.Title
                           select article;
            switch(sort)
            {
                case 1: // sort by title
                    articles = articles.OrderBy(a => a.Title);
                    break;
                case 2: // sort by date created
                    articles = articles.OrderBy(a => a.CreationDate);
                    break;
            }
            // it only happens when the argument is "true" (url)
            if (desc)
                articles.Reverse();

            ViewBag.Articles = articles;
            ViewBag.Sort = sort;
            ViewBag.Desc = desc;

            var sorts = new Dictionary<string, int>();
            sorts.Add("Title", 1);
            sorts.Add("Date Created", 2);
            ViewBag.Sorts = sorts;

            var order = new Dictionary<string, bool>();
            order.Add("ascending", false);
            order.Add("descending", true);
            ViewBag.Order = order;

            //TODO for now we search for them in the database, but later we include virtually in the article
            ViewBag.Categories = from category in db.Categories
                                      orderby category.Name
                                      select category;

            return View();
        }

        [HttpPost]
        public ActionResult Show(int sort, bool order)
        {
            ViewBag.Message = "Homepage";
            return RedirectToAction("Index", new { Sort = sort, Order = order });
        }


        public ActionResult Category(int categoryId = 0)
        {
            ViewBag.Message = "Articles from category";

            if (categoryId == 0)
                return RedirectToRoute("HomeCategory");
           
            ViewBag.Articles = from article in db.Articles
                               orderby article.Title
                               where article.CategoryId == categoryId
                               select article;
            // TODO, find the way of checking if there are any articles:
            ViewBag.hasArticles = true;
            //if (ViewBag.Articles.ToArray().GetLength() > 0)
            //    ViewBag.hasArticles = true;
            //else
            //    ViewBag.hasArticles = false;

            ViewBag.CategoryId = categoryId;
            var categories = from category in db.Categories
                             where category.CategoryId == categoryId
                             select category;
            ViewBag.CategoryName = categories.ToArray()[0].Name;
           
            return View();
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