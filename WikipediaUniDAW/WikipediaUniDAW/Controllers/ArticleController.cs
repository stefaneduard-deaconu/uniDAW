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

            if (TempData.ContainsKey("articleMessage")) {
                ViewBag.articleMessage = TempData["articleMessage"].ToString();
            }

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

            return View(new Article());
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

            if (TempData.ContainsKey("articleShowMessage")) {
                ViewBag.articleShowMessage = TempData["articleShowMessage"].ToString();
            }

            Article article = (from art in db.Articles
                              where art.ArticleId == id
                              select art).ToArray()[0];

            return View(article);
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

        [HttpGet]
        public ActionResult Edit(int id) {

            Article article = db.Articles.Find(id);
            article.Categories = GetAllCategories();

            if (User.IsInRole("Administrator") ||
                    (article.CreatorUserId == User.Identity.GetUserId() || User.IsInRole("Moderator")) && 
                    (!article.Frozen )
            ) {
                return View(article);
            } else {
                TempData["articleShowMessage"] = "You are not allowed to edit this article's details!";
                return RedirectToAction("Show", new { id = id });
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Article requestArticle) {
            try {
                if (ModelState.IsValid) {
                    Article article = db.Articles.Find(id);
                    if (TryUpdateModel(article)) {
                        article.CategoryId = requestArticle.CategoryId;
                        article.Title = requestArticle.Title;
                        article.Protected = requestArticle.Protected;
                        article.Frozen = requestArticle.Frozen;
                        TempData["articleShowMessage"] = "The article details have been modified!";
                        db.SaveChanges();
                    }
                    return RedirectToAction("Show", new { id = id });
                } else {
                    return View(requestArticle);
                }
            }
            catch (Exception e) {
                return View(requestArticle);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id) {

            Article article= db.Articles.Find(id);
            Category category = article.Category;
            db.Articles.Remove(article);
            TempData["articleMessage"] = "The article has been deleted!";
            db.SaveChanges();

            return RedirectToRoute("Articles of category", new { categoryId = category.CategoryId, sortingCriteria = 1 });
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