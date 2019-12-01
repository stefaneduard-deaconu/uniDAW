using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using WikipediaUniDAW.Models;

[assembly: OwinStartupAttribute(typeof(WikipediaUniDAW.Startup))]
namespace WikipediaUniDAW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            CreateAdminEditorRegularUsersAndApplicationRoles();
        }

        private void CreateAdminEditorRegularUsersAndApplicationRoles() {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            // add application roles
            if (!roleManager.RoleExists("Administrator")) {
                
                // add administrator role
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);
                
                // add the admin user
                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                var adminCreated = UserManager.Create(user, "12wq!@WQ");
                if (adminCreated.Succeeded) {
                    UserManager.AddToRole(user.Id, "Administrator");
                }
            }

            if (!roleManager.RoleExists("Moderator")) {

                // add moderator role
                var role = new IdentityRole();
                role.Name = "Moderator";
                roleManager.Create(role);

                // add a moderator user
                var user = new ApplicationUser();
                user.UserName = "moderator@moderator.com";
                user.Email = "moderator@moderator.com";
                var moderatorCreated = UserManager.Create(user, "12wq!@WQ");
                if (moderatorCreated.Succeeded) {
                    UserManager.AddToRole(user.Id, "Moderator");
                }
            }

            if (!roleManager.RoleExists("User")) {

                // add user role
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

                // add a regular user
                var user = new ApplicationUser();
                user.UserName = "user@user.com";
                user.Email = "user@user.com";
                var userCreated = UserManager.Create(user, "12wq!@WQ");
                if (userCreated.Succeeded) {
                    UserManager.AddToRole(user.Id, "User");
                }
            }
        }
    }
}
