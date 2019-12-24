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
        [Authorize]
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
        [Authorize]
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

            // check user permissions
            if (DenyUserPermission(article, "This article is frozen! You are not allowed to add new chapters.")) {
                return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
            }

            chapter.Version = article.CurrentVersion;
            chapter.VersionId = article.CurrentVersion.VersionId;

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

                    // check user permissions
                    if (DenyUserPermission(article, "This article is frozen! You are not allowed to add new chapters.")) {
                        return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
                    }

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

                    TempData["articleShowMessage"] = "New chapter has been added.";

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
        [Authorize]
        public ActionResult EditChapterForNewArticle(int chapterId, int articleId) {

            Chapter chapter = db.Chapters.Find(chapterId);

            return View(chapter);
        }

        [HttpPut]
        [Authorize]
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

        [HttpGet]
        public ActionResult EditChapterForExistingArticle(int id) {
            Chapter chapter = db.Chapters.Find(id);

            // check user permissions
            if (DenyUserPermission(chapter.Version.Article, "This article is frozen! You are not allowed to edit its chapters.")) {
                return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = chapter.Version.ArticleId });
            }

            ViewBag.DescriptionChange = "";

            return View(chapter);
        }

        [HttpPut]
        [ValidateInput(false)]
        public ActionResult EditChapterForExistingArticle(Chapter chapter, string descriptionChange) {

            try {
                if (!ModelState.IsValid) {
                    return View(chapter);
                }

                Models.Version oldVersion = (from ver in db.Versions
                                             where ver.VersionId == chapter.VersionId
                                             select ver).ToArray()[0];
                Article article = oldVersion.Article;
                Chapter[] chaptersOfOldVersion = oldVersion.Chapters.ToArray();

                // check user permissions
                if (DenyUserPermission(article, "This article is frozen! You are not allowed to edit its chapters.")) {
                    return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
                }

                // remove old associations
                article.CurrentVersionId = null;
                article.CurrentVersion = null;
                oldVersion.CurrentArticle = null;
                db.SaveChanges();

                // create new version
                Models.Version newVersion = new Models.Version {
                    ArticleId = article.ArticleId,
                    ModifierUserId = User.Identity.GetUserId(),
                    VersionNo = oldVersion.VersionNo + 1,
                    DateChange = DateTime.Now,
                    DescriptionChange = descriptionChange,
                    CurrentArticle = new Article[] { article },
                };

                db.Versions.Add(newVersion);
                db.SaveChanges();

                article.CurrentVersion = newVersion;
                db.SaveChanges();

                // copy all the chapters of the old version into the new version, editing the needed one
                foreach (Chapter chapterOfOldVersion in chaptersOfOldVersion) {

                    if (chapterOfOldVersion.ChapterId == chapter.ChapterId) {
                        db.Chapters.Add(new Chapter {
                            VersionId = newVersion.VersionId,
                            Title = chapter.Title,
                            Content = chapter.Content
                        });
                        db.SaveChanges();
                        continue;
                    }

                    db.Chapters.Add(new Chapter {
                        VersionId = newVersion.VersionId,
                        Title = chapterOfOldVersion.Title,
                        Content = chapterOfOldVersion.Content
                    });
                    db.SaveChanges();
                }

                TempData["articleShowMessage"] = "The chapter has been modified.";

                newVersion.ChangedChapterId = chapter.ChapterId;
                db.SaveChanges();

                if (TryUpdateModel(chapter)) {
                    chapter.AffectedVersion = new Models.Version[] { newVersion };
                    db.SaveChanges();
                }

                return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });

            } catch (Exception e) {
                return View(chapter);
            }
        }

        [HttpDelete]
        [Authorize]
        public ActionResult DeleteChapterForNewArticle(int chapterId, int articleId) {

            Chapter chapter = db.Chapters.Find(chapterId);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            TempData["versionMessage"] = "The chapter has been removed.";

            return RedirectToRoute("New version for new article", new { articleId = articleId });
        }

        [HttpDelete]
        public ActionResult DeleteChapterForExistingArticle(int id) {

            Chapter chapterToDelete = db.Chapters.Find(id);
            Models.Version oldVersion = chapterToDelete.Version;
            Article article = oldVersion.Article;
            Chapter[] chaptersOfOldVersion = oldVersion.Chapters.ToArray();

            // check user permissions
            if (DenyUserPermission(article, "This article is frozen! You are not allowed to delete its chapters.")) {
                return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
            }

            // remove old associations
            article.CurrentVersionId = null;
            article.CurrentVersion = null;
            oldVersion.CurrentArticle = null;
            db.SaveChanges();

            // create new version
            Models.Version newVersion = new Models.Version {
                ArticleId = article.ArticleId,
                ModifierUserId = User.Identity.GetUserId(),
                VersionNo = oldVersion.VersionNo + 1,
                DateChange = DateTime.Now,
                DescriptionChange = "Removed chapter '" + chapterToDelete.Title + "'",
                CurrentArticle = new Article[] { article },
            };

            db.Versions.Add(newVersion);
            db.SaveChanges();

            article.CurrentVersion = newVersion;
            db.SaveChanges();

            // copy all the chapters of the old version, excepting the one to delete, into the new version
            foreach (Chapter chapterOfOldVersion in chaptersOfOldVersion) {

                if (chapterOfOldVersion.ChapterId == chapterToDelete.ChapterId) {
                    continue;
                }

                db.Chapters.Add(new Chapter {
                    VersionId = newVersion.VersionId,
                    Title = chapterOfOldVersion.Title,
                    Content = chapterOfOldVersion.Content
                });
                db.SaveChanges();
            }

            newVersion.ChangedChapterId = chapterToDelete.ChapterId;
            newVersion.ChangedChapter = chapterToDelete;
            db.SaveChanges();

            TempData["articleShowMessage"] = "The chapter has been removed.";

            return RedirectToRoute("Default", new { controller = "Article", action = "Show", id = article.ArticleId });
        }

        [NonAction]
        private void CopyChaptersFromOldVersionToNewVersion(Models.Version oldVersion, Models.Version newVersion) {

            Chapter[] chaptersOfOldVersion = oldVersion.Chapters.ToArray();

            foreach (Chapter chapterOfOldVersion in chaptersOfOldVersion) {
                db.Chapters.Add(new Chapter {
                    VersionId = newVersion.VersionId,
                    Title = chapterOfOldVersion.Title,
                    Content = chapterOfOldVersion.Content
                });
                db.SaveChanges();
            }
        }

        [NonAction]
        private bool DenyUserPermission(Article article, String frozenArticleMessage) {

            if ( article.Frozen &&
                 !User.IsInRole("Administrator")
            ) {
                TempData["articleShowMessage"] = frozenArticleMessage;
                return true;
            }

            if ( !User.IsInRole("Administrator") &&
                 !User.IsInRole("Moderator") &&
                 !User.IsInRole("User") &&
                 article.Protected 
            ) {
                TempData["articleShowMessage"] = "This article is protected against unregistered users!";
                return true;
            }

            return false;
        }
    }
}