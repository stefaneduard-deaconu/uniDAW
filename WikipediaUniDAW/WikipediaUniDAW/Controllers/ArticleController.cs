using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(int categoryId, int sortingCriteria) {

            IOrderedQueryable<Article> articles;
            switch (sortingCriteria) {
                // select articles sorted by title
                case 1:
                    articles = from article in db.Articles
                               where article.CategoryId == categoryId
                               orderby article.Title
                               select article;
                    break;
                // select articles sorted by date
                case 2:
                    articles = from article in db.Articles
                               where article.CategoryId == categoryId
                               orderby article.CreationDate descending
                               select article;
                    break;
                default:
                    articles = null;
                    break;
            }

            ViewBag.Articles = articles;
            ViewBag.CategoryName = GetCategoryName(categoryId);

            // TODO: use ViewBag.IntroChapters array to store first chapter of each article to be displayed

            return View();
        }

        /**
         * Show an article with its chapters, giving possibility to: 
         * - edit article details (title, protection, freeze)
         * - add a new chapter to the article
         * - edit/delete any chapter
         * - delete the whole article
         * - show a list with all article's versions
         */
        [HttpGet]
        public ActionResult Show(int id) {
            
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult New() {

            Article article = new Article();
            article.Categories = GetAllCategories();
            article.CreatorUserId = User.Identity.GetUserId();

            return View(article);
        }

        [HttpPost]
        [Authorize]
        public ActionResult New(Article article) {
            //return RedirectToAction("Index", new { categoryId = article.CategoryId, sortingCriteria = 1 });
            article.Categories = GetAllCategories();

            try {
                if (ModelState.IsValid) {
                    db.Articles.Add(article);
                    db.SaveChanges();
                    return RedirectToRoute("New version for new article", new { articleId = article.ArticleId });
                } else {
                    return View(article);
                }
            }
            catch (Exception e) {
                return View(article);
            }
        }

        [NonAction]
        public string GetCategoryName(int categoryId) {
            return (from category in db.Categories
                    where category.CategoryId == categoryId
                    select category.Name).ToArray()[0];
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories() {
            var selectList = new List<SelectListItem>();

            var categories = from category in db.Categories
                             select category;

            foreach (var category in categories) {
                selectList.Add(new SelectListItem {
                    Value = category.CategoryId.ToString(),
                    Text = category.Name.ToString()
                });
            }

            return selectList;
        }
    }
}