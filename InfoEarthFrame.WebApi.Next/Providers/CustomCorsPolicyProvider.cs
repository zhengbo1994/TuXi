using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace InfoEarthFrame.WebApi.Next.Providers
{
    public class CustomCorsPolicyProvider : ICorsPolicyProvider
    {
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var corsPolicy=new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true
                };

                corsPolicy.Headers.Add(ConfigContext.Current.DefaultConfig["CurrentLangKey"]);

                var origins = ConfigContext.Current.DefaultConfig["AllowOrigins"].Split(',');
                foreach (var origin in origins)
                {
                    corsPolicy.Origins.Add(origin);
                }
                return corsPolicy;
            });
        }
    }
}