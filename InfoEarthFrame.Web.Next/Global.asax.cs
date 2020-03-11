using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using System.Security;
using System.IO;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Application.SystemUserApp;
using Abp.EntityFramework;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using log4net.Config;
using log4net;

[assembly: SecurityRules(SecurityRuleSet.Level1)]  
namespace InfoEarthFrame.Web.Next
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static IContainer DIContainer;
        private readonly ILog _logger = LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //使用依赖注入
            this.UseAutofac();

            //使用autoMapper
            this.UseAutoMapper();

            //使用log4net
            this.UseLog4net();

        }

        protected void Application_Error()
        {
            var ex = HttpContext.Current.Server.GetLastError();
            _logger.Error(ex);
        }
        private void UseAutofac()
        {

            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterType<InfoEarthFrameDbContext>();
            builder.RegisterType <SimpleDbContextProvider<InfoEarthFrameDbContext>>().As<IDbContextProvider<InfoEarthFrameDbContext>>();

            var repository = System.Reflection.Assembly.Load("InfoEarthFrame.EntityFramework");
            builder.RegisterAssemblyTypes(repository, repository)
              .AsImplementedInterfaces();

            var service = System.Reflection.Assembly.Load("InfoEarthFrame.Application");
            builder.RegisterAssemblyTypes(service, service)
              .AsImplementedInterfaces();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            DIContainer = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(DIContainer));
        }

        private void UseAutoMapper()
        {
           DtoMappings.Map();
        }

        protected void UseLog4net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\configs\log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

    }
}