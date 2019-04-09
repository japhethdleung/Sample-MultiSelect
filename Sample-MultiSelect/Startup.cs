using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample_MultiSelect.Startup))]
namespace Sample_MultiSelect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
