using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WelcomeTo.Startup))]
namespace WelcomeTo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
