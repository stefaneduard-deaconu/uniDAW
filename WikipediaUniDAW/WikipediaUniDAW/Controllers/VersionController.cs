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

        public ActionResult Index(int articleId) {

            var versions = from ver in db.Versions
                           where ver.ArticleId == articleId
                           orderby ver.DateChange descending
                           select ver;

            var article = (from art in db.Articles
                               where art.ArticleId == articleId
                               select art).ToArray()[0];

            ViewBag.Versions = versions;
            ViewBag.Article = article;

            return View();
        }

        [HttpGet]
        public ActionResult NewVersionForNewArticle(int articleId) {

            DeleteEmptyChaptersFromDataBase();

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

                    TempData["articleMessage"] = "New article has been created.";
                    return RedirectToRoute("Articles of category", new { categoryId = realVersion.Article.CategoryId, sortingCriteria = 1 });
                } else {
                    return View(version);
                }
            }
            catch (Exception e) {
                return View(version);
            }
        }

        [HttpPut]
        public ActionResult Revert(int id) {
            var desiredVersion = (from ver in db.Versions
                                  where ver.VersionId == id
                                  select ver).ToArray()[0];

            var article = desiredVersion.Article;

            var lastVersion = article.CurrentVersion;

            var subsequentVersions = (from ver in db.Versions
                                      where ver.ArticleId == desiredVersion.ArticleId &&
                                      ver.VersionNo > desiredVersion.VersionNo
                                      orderby ver.VersionNo descending
                                      select ver).ToArray();

            // change old connections
            article.CurrentVersionId = id;
            article.CurrentVersion = desiredVersion;
            lastVersion.CurrentArticle = null;
            db.SaveChanges();

            // delete the subsequent versions to the desired one
            foreach (var versionToDelete in subsequentVersions) {
                versionToDelete.ChangedChapterId = null;
                versionToDelete.ChangedChapter = null;
                db.SaveChanges();
                db.Versions.Remove(versionToDelete);
                db.SaveChanges();
            }

            TempData["articleShowMessage"] = "Article has been reverted to a previous version.";

            return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
        }

        [NonAction]
        public void DeleteEmptyChaptersFromDataBase() {
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