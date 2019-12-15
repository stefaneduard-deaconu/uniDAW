using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers
{
    public class ChapterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult NewChapterForNewArticle(int articleId) {

            Article article = (from art in db.Articles
                          where art.ArticleId == articleId
                          select art).ToArray()[0];

            Models.Version version = (from ver in db.Versions
                               where ver.VersionId == article.CurrentVersionId
                               select ver).ToArray()[0];

            Chapter chapter = new Chapter {
                VersionId = version.VersionId,
                Version = version,
                Title = "None",
                Content = "None"
                //AffectedVersion = new Models.Version[] { version }
            };

            db.Chapters.Add(chapter);
            db.SaveChanges();

            return View(chapter);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewChapterForNewArticle(Chapter chapter) {

            try {
                if (ModelState.IsValid) {
                    Chapter realChapter = db.Chapters.Find(chapter.ChapterId);

                    if (TryUpdateModel(realChapter)) {
                        realChapter.Title = chapter.Title;
                        realChapter.Content = chapter.Content;
                        // using a Sanitizer may cause specific image urls not to save in the database
                        //realChapter.Content = Sanitizer.GetSafeHtmlFragment(chapter.Content);
                        db.SaveChanges();
                        TempData["versionMessage"] = "New chapter added!";
                    }
                    return RedirectToRoute("New version for new article", new { articleId = realChapter.Version.Article.ArticleId });
                } else {
                    return View(chapter);
                }
            }
            catch (Exception e) {
                return View(chapter);
            }
        }

        [HttpGet]
        public ActionResult NewChapterForExistingArticle(int articleId) {
            Chapter chapter = new Chapter();

            Article article = (from art in db.Articles
                               where art.ArticleId == articleId
                               select art).ToArray()[0];

            chapter.Version = article.CurrentVersion;
            chapter.VersionId = article.CurrentVersion.VersionId;

            //ViewBag.ArticleId = article.ArticleId;

            //chapter.Version = (from version in db.Versions
            //                  where (from art in version.CurrentArticle select art.ArticleId).ToArray().Contains(articleId)
            //                  select version).ToArray()[0];
            return View(chapter);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewChapterForExistingArticle(Chapter chapter) {

            try {
                if (ModelState.IsValid) {
                    
                    // get old entities
                    Models.Version oldVersion = (from ver in db.Versions
                                                 where ver.VersionId == chapter.VersionId
                                                 select ver).ToArray()[0];

                    Article article = (from art in db.Articles
                                       where art.ArticleId == oldVersion.ArticleId
                                       select art).ToArray()[0];
                    
                    Chapter[] chaptersOfOldVersion = (from chap in db.Chapters
                                                      where chap.VersionId == oldVersion.VersionId
                                                      select chap).ToArray();

                    // remove old associations
                    article.CurrentVersionId = null;
                    article.CurrentVersion = null;
                    oldVersion.CurrentArticle = null;
                    db.SaveChanges();

                    // create new entities
                    Models.Version newVersion = new Models.Version {
                        ArticleId = article.ArticleId,
                        ModifierUserId = User.Identity.GetUserId(),
                        VersionNo = oldVersion.VersionNo + 1,
                        DateChange = DateTime.Now,
                        DescriptionChange = "Added chapter '" + chapter.Title + "'",
                        CurrentArticle = new Article[] { article },
                    };

                    db.Versions.Add(newVersion);
                    db.SaveChanges();

                    article.CurrentVersion = newVersion;
                    db.SaveChanges();

                    // create chapters for new version
                    foreach (Chapter chapterOfOldVersion in chaptersOfOldVersion) {
                        db.Chapters.Add(new Chapter {
                            VersionId = newVersion.VersionId,
                            Title = chapterOfOldVersion.Title,
                            Content = chapterOfOldVersion.Content
                        });
                        db.SaveChanges();
                    }

                    Chapter newChapter = new Chapter {
                        VersionId = newVersion.VersionId,
                        Title = chapter.Title,
                        Content = chapter.Content,
                        AffectedVersion = new Models.Version[] { newVersion }
                    };
                    db.Chapters.Add(newChapter);
                    db.SaveChanges();

                    return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
                } else {
                    return View(chapter);
                }
            }
            catch (Exception e) {
                return View(chapter);
            }

            
        }

        [HttpGet]
        public ActionResult EditChapterForNewArticle(int chapterId, int articleId) {

            Chapter chapter = db.Chapters.Find(chapterId);

            return View(chapter);
        }

        [HttpPut]
        [ValidateInput(false)]
        public ActionResult EditChapterForNewArticle(Chapter chapter) {

            try {
                if (ModelState.IsValid) {
                    Chapter realChapter = db.Chapters.Find(chapter.ChapterId);

                    if (TryUpdateModel(realChapter)) {
                        realChapter.Title = chapter.Title;
                        realChapter.Content = chapter.Content;
                        // using a Sanitizer may cause specific image urls not to save in the database
                        //realChapter.Content = Sanitizer.GetSafeHtmlFragment(chapter.Content);
                        db.SaveChanges();
                        TempData["versionMessage"] = "The chapter has been modified!";
                    }
                    return RedirectToRoute("New version for new article", new { articleId = realChapter.Version.Article.ArticleId });
                } else {
                    return View(chapter);
                }
            }
            catch (Exception e) {
                return View(chapter);
            }
        }

        [HttpDelete]
        public ActionResult DeleteChapterForNewArticle(int chapterId, int articleId) {

            Chapter chapter = db.Chapters.Find(chapterId);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            TempData["versionMessage"] = "The chapter has been deleted!";

            return RedirectToRoute("New version for new article", new { articleId = articleId });
        }
    }
}