using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(hamroktmMainClient.Startup))]
namespace hamroktmMainClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
