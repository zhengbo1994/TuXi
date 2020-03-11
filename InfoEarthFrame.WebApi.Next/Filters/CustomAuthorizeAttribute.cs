
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using InfoEarthFrame.WebApi.Next.Models;
using InfoEarthFrame.WebApi.Next.Config;

namespace InfoEarthFrame.WebApi.Next.Filters
{
    public class CustomAuthorizeAttribute:AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            var flag = ApiAuthorizeManager.IsPageValid(context.Request.RequestUri.LocalPath, () =>
            {
                return context.Request.GetOwinContext().Authentication.User.Identity.IsAuthenticated;
            });
            if (!flag)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                resp.Content = new StringContent(JsonConvert.SerializeObject(new ApiResult
                {
                    Code = 401,
                    Message = "Unauthorized"
                }));
                context.Response = resp;
            }
        }
    }
}