using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikipediaUniDAW.Models;

namespace WikipediaUniDAW.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        [HttpGet]
        public ActionResult Index() {
            // keep track of printing the temporary message related to editing / removing a user
            if (TempData.ContainsKey("userCrudMessage")) {
                ViewBag.userCrudMessage = TempData["userCrudMessage"].ToString();
            }

            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;
            return View();
        }

        [HttpGet]
        public ActionResult Show(string id) {
            ApplicationUser user = db.Users.Find(id);
            ViewBag.UserRoleName = GetUserRoleName(user);
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(string id) {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.UserRole = userRole.RoleId;
            return View(user);
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newUserData) {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.UserRole = userRole.RoleId;
            try {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                if (TryUpdateModel(user)) {
                    user.UserName = newUserData.UserName;
                    user.Email = newUserData.Email;
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles) {
                        userManager.RemoveFromRole(id, role.Name);
                    }
                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newAssignedRole"));
                    userManager.AddToRole(id, selectedRole.Name);
                    TempData["userCrudMessage"] = "The user has been saved!";
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e) {
                Response.Write(e.Message);
                return View(user);
            }
        }

        [HttpDelete]
        public ActionResult Delete(string id) {
            ApplicationUser user = db.Users.Find(id);

            if (GetUserRoleName(user) == "Administrator") {
                return RedirectToAction("Index");
            }

            db.Users.Remove(user);
            TempData["userCrudMessage"] = "The user has been deleted!";
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles() {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles) {
                selectList.Add(new SelectListItem {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [NonAction]
        public string GetUserRoleName(ApplicationUser user) {
            var userRole = user.Roles.FirstOrDefault();
            return (from role in db.Roles
                    where role.Id == userRole.RoleId
                    select role.Name).ToArray()[0];
        }
    }
}