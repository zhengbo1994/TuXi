using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Filters
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true,Inherited=true)]
    public class CustomAuthFilterAttribute:System.Attribute,System.Web.Mvc.IAuthorizationFilter
    {
        public void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (!UserIdentityContext.IsLogin)
            {
                if (!ConfigContext.Current.AuthConfig.IsValid(filterContext))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "account",
                        action = "login",
                        returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl)
                    }));
                }
            }
        }
    }
}