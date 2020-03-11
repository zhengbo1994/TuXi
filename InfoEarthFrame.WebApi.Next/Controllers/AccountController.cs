using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Common;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.WebApi.Next.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class AccountController : BaseApiController
    {
        public readonly ISystemUserAppService _systemUserAppService;
        public AccountController(ISystemUserAppService systemUserAppService)
        {
            this._systemUserAppService = systemUserAppService;
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldpassword">旧密码</param>
        /// <param name="password">新密码</param>
        /// <param name="repassword">确认新密码</param>
        /// <param name="currentUserName">账号</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetChangePassword(string oldpassword, string password, string repassword, string currentUserName)
        {
            var verifyflag = _systemUserAppService.GetLoginPasswordByUserCode(oldpassword, currentUserName);
            if (verifyflag == false)
            {
                return Ok(GetResult(verifyflag, "原密码错误！"));
            }

            var flag = await _systemUserAppService.ChangePassword(password, currentUserName);
            return Ok(GetResult(flag, "修改成功！"));
        }

        public IHttpActionResult GetPermession()
        { 
            var permession=new List<Module>();
            if (CurrentUserName.ToLower() == "admin")
            {
                ConfigContext.Current.ModuleConfig.Modules.ForEach(m =>
                {
                        if (!permession.Exists(j => j.ID == m.ID))
                        {
                            permession.Add(m);
                        }
                });
                return Ok(GetResult(0, permession.OrderBy(p => p.ID).ToList()));
            }
            using (var db = new InfoEarthFrameDbContext())
            {
                var groupIds = db.GroupUserEntities.Where(p => p.UserId == CurrentUserId).Select(p => p.GroupId).ToList();

                ConfigContext.Current.PermessionConfig.Groups.Where(p => groupIds.Contains(p.ID)).Select(p => p.Modules).ToList().ForEach(p =>
                {
                    p.ForEach(m =>
                    {
                        if (!permession.Exists(j => j.ID==m.ID))
                        {
                            permession.Add(m);
                        }
                    });
                });
            }

            return Ok(GetResult(0, permession.OrderBy(p => p.ID).ToList()));
        }

        public IHttpActionResult GetPagePermession(string menuUrl)
        {
            var data= ConfigContext.Current.PermessionConfig.GetMenuButtonPermession(CurrentUserGroupIds, menuUrl);
            return Ok(GetResult(0, data));
        }
    }
}