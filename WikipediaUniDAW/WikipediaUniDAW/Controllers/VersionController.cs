using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers
{
    public class VersionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index() {

            return View();
        }

        [HttpGet]
        public ActionResult NewVersionForNewArticle(int articleId) {

            if (TempData.ContainsKey("versionMessage")) {
                ViewBag.versionMessage = TempData["versionMessage"].ToString();
            }

            Models.Version[] foundVersion = (from ver in db.Versions
                                             where ver.ArticleId == articleId
                                             select ver).ToArray();

            if (foundVersion.Length != 0) {
                // we have already created a first version for this article
                return View(foundVersion[0]);
            }

            Article article = (from art in db.Articles
                          where art.ArticleId == articleId
                          select art).ToArray()[0];

            Models.Version version = new Models.Version {
                ArticleId = articleId,
                Article = article,
                ModifierUserId = User.Identity.GetUserId(),
                ChangedChapterId = null,
                ChangedImageId = null,
                VersionNo = 1,
                DescriptionChange = "Article Creation",
                CurrentArticle = new Article[] { article },
                Chapters = new Chapter[] { }
            };

            db.Versions.Add(version);

            if (TryUpdateModel(article)) {
                article.CurrentVersionId = version.VersionId;
            }

            db.SaveChanges();

            return View(version);
        }

        [HttpPost]
        public ActionResult NewVersionForNewArticle(Models.Version version) {

            DeleteEmptyChaptersFromDataBase();

            try {
                if (ModelState.IsValid) {

                    Models.Version realVersion = db.Versions.Find(version.VersionId);

                    Chapter[] chaptersOfVersion = (from chapter in db.Chapters
                                                   where chapter.VersionId == version.VersionId
                                                   select chapter).ToArray();

                    if (chaptersOfVersion.Length == 0) {
                        TempData["versionMessage"] = "Please create at least one chapter for the new article.";
                        ViewBag.versionMessage = TempData["versionMessage"].ToString();
                        return View(realVersion);
                    }

                    if (TryUpdateModel(realVersion)) {
                        realVersion.Chapters = chaptersOfVersion;
                        realVersion.DateChange = DateTime.Now;
                        db.SaveChanges();
                    }
                    
                    return RedirectToRoute("Articles of category", new { categoryId = realVersion.Article.CategoryId, sortingCriteria = 1 });
                } else {
                    return View(version);
                }
            }
            catch (Exception e) {
                return View(version);
            }
        }

        [NonAction]
        void DeleteEmptyChaptersFromDataBase() {
            var emptyChapters = from chapter in db.Chapters
                                where chapter.Title == "None" && chapter.Content == "None"
                                select chapter;

            foreach (var emptyChapter in emptyChapters) {
                db.Chapters.Remove(emptyChapter);
            }

            db.SaveChanges();
        }
    }
}