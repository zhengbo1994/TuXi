using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ISystemUserAppService _systemUserAppService;
        public AccountController(ISystemUserAppService systemUserAppService)
        {
            this._systemUserAppService = systemUserAppService;
        }


        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            var result = new HttpResponseResult();
            var url = ConfigContext.Current.ApiConfig["GetAccessToken"];
            var errMsg = "";
            var apiResp = _systemUserAppService.GetAccessToken(username, password, GetApiUrl(url),out errMsg);
            if (!string.IsNullOrEmpty(errMsg) || apiResp.Contains("Error"))
            {
                result.Code = 500;
                result.Message = errMsg;
                return Json2(result);
            }

            OauthToken authToken=null;
            try
            {
                 authToken = JsonConvert.DeserializeObject<OauthToken>(apiResp);
            }
            catch (Exception ex)
            {
                result.Code = -200;
                result.Message = ex.Message;
                return Json2(result);
            }

            if (authToken==null|| string.IsNullOrEmpty(authToken.AccessToken))
            {
                result.Code = -200;
                return Json2(result);
            }

            if (authToken != null)
            {
                result.Code = 0;
                //登录成功
                UserIdentityContext.Login(new LoginUser
                {
                    Name = username,
                    Password = password,
                    AccessToken = authToken.AccessToken
                });
            }
            return Json2(result);

        }

        public ActionResult Logout()
        {
            UserIdentityContext.Logout();
            return RedirectToAction("login");
        }

        public ActionResult GetTokenTest()
        {
            return View();
        }

        public ActionResult Changepassword()
        {
            return View();
        }

        
    }
}
