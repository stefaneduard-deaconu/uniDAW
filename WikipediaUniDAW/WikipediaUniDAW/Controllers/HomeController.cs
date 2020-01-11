using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers {
    public class HomeController : Controller {

        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index(int sort = 0, int order = 0) {
            ViewBag.Message = "Homepage";

            //var articles = from article in db.Articles
            //               select article;
            IOrderedQueryable<Article> articles = null;

            switch(sort)
            {
                case 0: // sort by title
                    if (order == 0) {
                        articles = from article in db.Articles
                                       orderby article.Title ascending
                                       select article;
                    } else {
                        articles = from article in db.Articles
                                   orderby article.Title descending
                                   select article;
                    }
                    //articles = articles.OrderBy(a => a.Title);
                    break;
                case 1: // sort by date created
                    if (order == 0) {
                        articles = from article in db.Articles
                                   orderby article.CreationDate ascending
                                   select article;
                    } else {
                        articles = from article in db.Articles
                                   orderby article.CreationDate descending
                                   select article;
                    }
                    //articles = articles.OrderBy(a => a.CreationDate);
                    break;
            }
            // it only happens when the argument is "true" (url)
            //if (order == 1)
            //    articles = articles.Reverse();

            ViewBag.Articles = articles;
            //ViewBag.Sort = sort;
            //ViewBag.Order = order;

            var sorts = new Dictionary<string, int>();
            sorts.Add("Title", 0);
            sorts.Add("Date Created", 1);
            ViewBag.Sorts = sorts;

            var orders = new Dictionary<string, string>();
            orders.Add("ascending", "0");
            orders.Add("descending", "1");
            ViewBag.Orders = orders;

            //TODO for now we search for them in the database, but later we include virtually in the article
            ViewBag.Categories = from category in db.Categories
                                      orderby category.Name
                                      select category;

            return View();
        }

        // TODO make this work out, as to sort the items :(
        [HttpPost]
        public ActionResult Index(string sort, string order)
        {
            ViewBag.Message = "Homepage redirect after sort";
            return RedirectToRoute("Home/" + sort + "/" + order);
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
            if (categories.Count() > 0)
            {
                ViewBag.CategoryName = categories.ToArray()[0].Name;
                ViewBag.isCategoryId = true;
            }
            else // TODO, this isn't used in the View, yet
                ViewBag.isCategoryId = false;

            return View();
        }

        public ActionResult Search(string queryString)
        {
            ViewBag.Query = queryString; 
            queryString = queryString.ToLower();

            var articles = (from article in db.Articles
                            select article).ToArray();

            var relatedWords = new List<string>();
            var goodArticles = new List<Article>();

            foreach (Article article in articles)
            {
                bool queryRelated = false;
                foreach (string word in article.Title.Split())
                {
                    if (queryString.Contains(word.ToLower()))
                    {
                        queryRelated = true;
                        relatedWords.Add(word);
                    }
                }
                if (queryRelated)
                    goodArticles.Add(article);
            }

            ViewBag.Articles = goodArticles;
            ViewBag.Words = relatedWords;
            return View();
        }
    }
}