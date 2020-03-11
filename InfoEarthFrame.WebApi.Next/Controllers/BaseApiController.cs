
using InfoEarthFrame.Common;
using InfoEarthFrame.WebApi.Next.Config;
using InfoEarthFrame.WebApi.Next.Filters;
using InfoEarthFrame.WebApi.Next.Models;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    [CustomHandleExceptionAttriute]
    [CustomAuthorize]
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// Api模块名称
        /// </summary>
        protected virtual string ModuleName
        {
            get {
               return "Base";
            }
        }

        protected virtual ApiResult GetResult(object key, object data = null)
        {
            return ApiContext.GetApiResult(ModuleName, key,data);
        }

        protected virtual string GetText(object key, object data = null)
        {
            return ApiContext.GetApiResult(ModuleName, key).Message;
        }

        protected IOwinContext MyContext
        {
             get
            {
                return Request.GetOwinContext();
            }
        }

        protected string CurrentLang
        {
            get
            {
                var lang= MyContext.Request.Headers[ConfigContext.Current.DefaultConfig["CurrentLangKey"]];
                if (string.IsNullOrEmpty(lang))
                {
                    return "zh-cn";
                }
                return lang;
            }
        }

        protected string CurrentUserId
        {
            get
            {
                var user = MyContext.Authentication.User.Claims.FirstOrDefault(p => p.Type == "userid");
                return user.Value;
            }
        }


        protected string CurrentUserName
        {
            get
            {
                var user = MyContext.Authentication.User.Claims.FirstOrDefault(p => p.Type == "username");
                return user.Value;
            }
        }

        protected string[] CurrentUserGroupIds
        {
            get
            {
                var user = MyContext.Authentication.User.Claims.FirstOrDefault(p => p.Type.ToLower().Contains("role"));
                return user.Value.Split('|');
            }
        }


        public HttpResponseMessage GetFile(string filePath,string fileName)
        {
            try
            {
                filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), HttpUtility.UrlDecode(filePath).Replace("/", "\\"));
                var stream = new FileStream(filePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlDecode(fileName)
                };
                return response;
            }
            catch
            {
                throw;
            }
        }

       
        //public HttpRequest Request
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request;
        //    }
        //}
    }
}
