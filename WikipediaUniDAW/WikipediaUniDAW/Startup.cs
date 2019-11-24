using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WikipediaUniDAW.Startup))]
namespace WikipediaUniDAW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
