using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyHealthClinic.Startup))]
namespace MyHealthClinic
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
