using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WikipediaUniDAW {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            // TODO a better routing
            routes.MapRoute(
                name: "HomeCategory",
                url: "Home/Category/{categoryId}",
                defaults: new
                {
                    controller = "Home",
                    action = "Category",
                    categoryId = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "Home",
                url: "Home/{sort}/{order}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    sort = UrlParameter.Optional,
                    order = UrlParameter.Optional
                }
            );
            
            
            


            // acum vad si eu ca erau implementate :D facepalm ;D
            routes.MapRoute(
                name: "Articles of category",
                url: "Article/Index/{categoryId}/{sortingCriteria}",
                defaults: new { controller = "Article", action = "Index" }
            );

            routes.MapRoute(
                name: "Versions of article",
                url: "Version/Index/{articleId}",
                defaults: new { controller = "Version", action = "Index" }
            );

            routes.MapRoute(
                name: "New version for new article",
                url: "Version/NewVersionForNewArticle/{articleId}",
                defaults: new { controller = "Version", action = "NewVersionForNewArticle" }
            );

            routes.MapRoute(
                name: "New chapter for new article",
                url: "Chapter/NewChapterForNewArticle/{articleId}",
                defaults: new { controller = "Chapter", action = "NewChapterForNewArticle" }
            );

            routes.MapRoute(
                name: "Edit chapter for new article",
                url: "Chapter/EditChapterForNewArticle/{chapterId}/{articleId}",
                defaults: new { controller = "Chapter", action = "EditChapterForNewArticle" }
            );

            routes.MapRoute(
                name: "Delete chapter for new article",
                url: "Chapter/DeleteChapterForNewArticle/{chapterId}/{articleId}",
                defaults: new { controller = "Chapter", action = "DeleteChapterForNewArticle" }
            );

            routes.MapRoute(
                name: "New chapter for existing article",
                url: "Chapter/NewChapterForExistingArticle/{articleId}",
                defaults: new { controller = "Chapter", action = "NewChapterForExistingArticle" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
