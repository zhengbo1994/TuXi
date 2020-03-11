using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Thinktecture.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using InfoEarthFrame.WebApi.Next.Controllers;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Owin.Security.Provider;
using InfoEarthFrame.WebApi.Next.Providers;
using Autofac;
using Autofac.Integration.WebApi;
using InfoEarthFrame.Core.Repositories;
using Abp.EntityFramework;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using System.Reflection;
using InfoEarthFrame.Common;
using InfoEarthFrame.WebApi.Next.Models;
using System.IO;
using log4net.Config;

[assembly: OwinStartup(typeof(InfoEarthFrame.WebApi.Next.MyStartup))]

namespace InfoEarthFrame.WebApi.Next
{
    public class MyStartup
    {
        public static IContainer DIContainer;
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

     
            ConfigureAuth(app);
            config.EnableCors(new CustomCorsPolicyProvider());

     

            app.UseWebApi(config);
            this.UseAutofac(config);
            this.UseAutoMapper();
            this.UseLog4net();
        }


        protected void UseAutofac(HttpConfiguration config)
        {

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<InfoEarthFrameDbContext>();
            builder.RegisterType<SimpleDbContextProvider<InfoEarthFrameDbContext>>().As<IDbContextProvider<InfoEarthFrameDbContext>>();

            var repository = System.Reflection.Assembly.Load("InfoEarthFrame.EntityFramework");
            builder.RegisterAssemblyTypes(repository, repository)
              .AsImplementedInterfaces();

            var service = System.Reflection.Assembly.Load("InfoEarthFrame.Application");
            builder.RegisterAssemblyTypes(service, service)
              .AsImplementedInterfaces();
            DIContainer = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(DIContainer);
        }

        protected void UseAutoMapper()
        {
            DtoMappings.Map();
        }

        protected void UseLog4net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\configs\log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            var issuer = ConfigContext.Current.DefaultConfig["auth:issuer"];
            var secret = TextEncodings.Base64Url.Decode(Convert.ToBase64String(
                System.Text.Encoding.Default.GetBytes(ConfigContext.Current.DefaultConfig["auth:secret"])));

            //用jwt进行身份认证
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { "Any" },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]{
                new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                }
            });


            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                //生产环境设为false
                AllowInsecureHttp = true,
                //请求token的路径
                TokenEndpointPath = new PathString("/api/oauth2/token"),
                //TODO:过期时间再看
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                //请求获取token时，验证username, password
                Provider = new CustomOAuthProvider(),
                //定义token信息格式 
                AccessTokenFormat = new CustomJwtFormat(issuer, secret)
            });

        }
    }

  
    /// <summary> 
    /// 自定义 jwt token 的格式 
    /// </summary>
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly byte[] _secret;
        private readonly string _issuer;

        public CustomJwtFormat(string issuer, byte[] secret)
        {
            _issuer = issuer;
            _secret = secret;
        }

        public string Protect(AuthenticationTicket data)
        {

            var signingKey = new HmacSigningCredentials(_secret);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_issuer, "Any", data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey));
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 自定义 jwt oauth 的授权验证
    /// </summary>
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        private static readonly List<string> AccessControlAllowOrigins =ConfigContext.Current.DefaultConfig["AllowOrigins"].Split(',').ToList();

        private void AddOrigin(IOwinRequest request)
        {
            var orgin = request.Headers["Origin"];
            if (AccessControlAllowOrigins.Exists(p => p == orgin))
            {
                request.Context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { orgin });
            }
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
           var orgin= HttpContext.Current.Request.Headers["Origin"];
            AddOrigin(context.Request);

            var username = context.UserName;
            var password = context.Password;
            string userid;
            string errMsg;
            string group;
            if (!CheckCredential(context, out userid,out errMsg,out group))
            {
                context.SetError(errMsg);
                context.Rejected();
                return Task.FromResult<object>(null);
            }
            var ticket = new AuthenticationTicket(SetClaimsIdentity(context, userid, username,group), new AuthenticationProperties());
            context.Validated(ticket);
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        private static ClaimsIdentity SetClaimsIdentity(OAuthGrantResourceOwnerCredentialsContext context, string userid, string usercode, string group)
        {
            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim("userid", userid));
            identity.AddClaim(new Claim("username", usercode));
            identity.AddClaim(new Claim("role", group));
            return identity;
        }

        private static bool CheckCredential(OAuthGrantResourceOwnerCredentialsContext context, out string userid,out string errMsg,out string group)
        {
            // 用户名和密码验证
            var username = context.UserName;
            var password = context.Password;

            using (var scope = MyStartup.DIContainer.BeginLifetimeScope())
            {
                var service = MyStartup.DIContainer.Resolve<ISystemUserAppService>();
                var model = service.GetDetailByNamePassword(new SystemUserDto
                {
                    UserCode = username,
                    Password = password
                });
                if (model == null || string.IsNullOrEmpty(model.Id))
                {
                    userid = "";
                    errMsg = "invalid username or password";
                    group = "";
                }
                else
                {
                    userid = model.Id;
                    errMsg = "";
                    group = string.Join("|", model.GroupIds??new string[0]);
                    return true;
                }
            }

            return false;
        }
    }
}
