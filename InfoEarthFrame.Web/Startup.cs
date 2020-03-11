//using IdentityModel.Client;
//using IdentityServer3.AccessTokenValidation;
//using Microsoft.IdentityModel.Protocols;
//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.OpenIdConnect;
//using Owin;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel;
//using System.IdentityModel.Tokens;
//using System.Linq;
//using System.Reflection;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;
//[assembly: OwinStartup(typeof(iTelluro.SSO.WebApp.Startup))]
//namespace iTelluro.SSO.WebApp
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {

//            TokenValidationParameters tokenParame = new TokenValidationParameters();
//            tokenParame.ValidateLifetime = false;

//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = "Cookies"
//            });

//            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
//            {

//                TokenValidationParameters = tokenParame, //忽略时间验证

//                Authority = Constants.Authority,//读取配置文件设置属性Authority
//                ClientId = Constants.ImplicitClient,//读取配置文件设置属性ClientId
//                Scope = "openid profile roles sampleApi",
//                ResponseType = "id_token token",
//                RedirectUri = Constants.RedirectUri,//读取配置文件设置属性RedirectUri
//                PostLogoutRedirectUri = Constants.PostLogoutRedirectUri,//读取配置文件设置属性PostLogoutRedirectUri

//                UseTokenLifetime = false,
//                SignInAsAuthenticationType = "Cookies",

//                Notifications = new OpenIdConnectAuthenticationNotifications
//                {
//                    AuthenticationFailed = async faildMsg =>
//                    {//n.ProtocolMessage.Error;
//                        if (faildMsg.Exception is OpenIdConnectProtocolInvalidNonceException)
//                        {
//                            if (faildMsg.Exception.Message.Contains("IDX10311"))
//                            {
//                                faildMsg.SkipToNextMiddleware();
//                            }
//                            if (faildMsg.Exception.Message.Contains("IDX10301"))
//                            {
//                                faildMsg.HandleResponse();
//                            }
//                        }
//                    },
//                    SecurityTokenValidated = async n =>
//                    {
//                        var nid = new ClaimsIdentity(
//                            n.AuthenticationTicket.Identity.AuthenticationType,
//                            "given_name",
//                            "role");

//                        // get userinfo data
//                        var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo"), n.ProtocolMessage.AccessToken);

//                        var userInfo = await userInfoClient.GetAsync();

//                        if (userInfo.Claims != null)
//                        {
//                            userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));
//                        }

//                        // keep the id_token for logout
//                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

//                        // add access token for sample API
//                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

//                        // keep track of access token expiration
//                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

//                        // add some other app specific claim
//                        nid.AddClaim(new Claim("app_specific", "some data"));

//                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);

//                    },

//                    RedirectToIdentityProvider = n =>
//                    {
//                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
//                        {
//                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

//                            if (idTokenHint != null)
//                            {
//                                n.ProtocolMessage.PostLogoutRedirectUri = Constants.PostLogoutRedirectUri;
//                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
//                            }
//                        }

//                        return Task.FromResult(0);
//                    }
//                }
//            });
//            app.Map("/api/services/app", inner =>
//            {
//                var bearerTokenOptions = new IdentityServerBearerTokenAuthenticationOptions
//                {
//                    Authority = Constants.Authority,
//                    RequiredScopes = new[] { "sampleApi" }
//                };

//                inner.UseIdentityServerBearerTokenAuthentication(bearerTokenOptions);
//            }
//            );
//        }
//    }
//}