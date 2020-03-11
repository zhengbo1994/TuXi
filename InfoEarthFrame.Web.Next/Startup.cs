using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
//using Infoearth.SignalR;

[assembly: OwinStartup(typeof(InfoEarthFrame.Web.Next.Startup))]

namespace InfoEarthFrame.Web.Next
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888
            //  app.UseCors(CorsOptions.AllowAll);
            //app.MapSignalR<MessageConnection>("/SignalR/MessageConnection");
        }
    }
}
