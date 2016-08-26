using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BIRA_Issue_Tracker.Startup))]
namespace BIRA_Issue_Tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
