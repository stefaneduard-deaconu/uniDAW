using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index() {
            // keep track of printing the temporary message related to adding / editing / removing a category
            if (TempData.ContainsKey("categoryMessage")) {
                ViewBag.categoryMessage = TempData["categoryMessage"].ToString();
            }

            var categories = from category in db.Categories
                             orderby category.Name
                             select category;

            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet]
        public ActionResult New() {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category category) {
            try {
                if (ModelState.IsValid) {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["categoryMessage"] = "Added new category to the list!";
                    return RedirectToAction("Index");
                } else {
                    return View(category);
                }
            }
            catch (Exception e) {
                return View(category);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id) {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory) {
            try {
                if (ModelState.IsValid) {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category)) {
                        category.Name = requestCategory.Name;
                        TempData["categoryMessage"] = "The category has been saved!";
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                } else {
                    return View(requestCategory);
                }
            }
            catch (Exception e) {
                return View(requestCategory);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id) {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["categoryMessage"] = "The category has been deleted!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}