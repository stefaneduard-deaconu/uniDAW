﻿using Microsoft.Security.Application;
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