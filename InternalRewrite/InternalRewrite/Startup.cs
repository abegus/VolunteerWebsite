using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InternalRewrite.Startup))]
namespace InternalRewrite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
