using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MFramework_Services_IdentityAuth.Startup))]
namespace MFramework_Services_IdentityAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
