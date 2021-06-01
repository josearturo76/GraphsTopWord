using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GraphsTopWord.Startup))]
namespace GraphsTopWord
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
