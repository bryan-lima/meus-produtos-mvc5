using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DevIO.AppMVC5.Startup))]
namespace DevIO.AppMVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
