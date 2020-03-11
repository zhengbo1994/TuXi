using InfoEarthFrame.WebApi.Next.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace InfoEarthFrame.WebApi.Next.Filters
{
    public class CustomHandleExceptionAttriute : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CustomAuthorizeAttribute));
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //记录日志
            _logger.Error(actionExecutedContext.Exception);

            var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            resp.Content = new StringContent(JsonConvert.SerializeObject(new ApiResult
        {
            Code = 500,
            Message = actionExecutedContext.Exception.Message
        }));
            actionExecutedContext.Response = resp;
        }
    }
}