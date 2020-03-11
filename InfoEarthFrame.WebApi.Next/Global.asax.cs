using Autofac;
using Autofac.Integration.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Security;
using System.IO;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Application.SystemUserApp;
using Abp.EntityFramework;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Application.SystemUserApp.Dtos;


namespace InfoEarthFrame.WebApi.Next
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
