using System.Reflection;
using Abp.Application.Services;
using Abp.Modules;
using Abp.WebApi;
using Abp.WebApi.Controllers.Dynamic.Builders;
using System.Web.Http;
using System.Web.Http.Cors;

namespace InfoEarthFrame
{
    [DependsOn(typeof(AbpWebApiModule), typeof(InfoEarthFrameApplicationModule))]
    public class InfoEarthFrameWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(InfoEarthFrameApplicationModule).Assembly, "app").WithConventionalVerbs()
                .Build();

            ////设置跨域
            //EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            //cors.SupportsCredentials = true;
            //GlobalConfiguration.Configuration.EnableCors(cors);
        }
    }
}
