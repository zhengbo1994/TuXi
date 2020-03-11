using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.Common
{
    public class UserIdentityContext
    {
        private static readonly string LoginUserKey = ConfigContext.Current.DefaultConfig["LoginUserKey"];
        private static readonly string AccessTokenKey = ConfigContext.Current.DefaultConfig["AccessTokenKey"];
        /// <summary>
        /// 当前登陆用户对象
        /// </summary>
        public static LoginUser Current
        {
            get
            {
                return (LoginUser)HttpContext.Current.Session[LoginUserKey];
            }
        }

        public static string CurrentUserName
        {
            get {
                return Current != null ? Current.Name : "";
            }
        }


        /// <summary>
        /// 是否登陆
        /// </summary>
        public static bool IsLogin
        {
            get
            {
                return Current != null;
            }
        }


        /// <summary>
        /// 登陆
        /// </summary>
        public static void Login(LoginUser user)
        {
            HttpContext.Current.Session[LoginUserKey] = user;
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(AccessTokenKey) { Expires = DateTime.Now.AddDays(30) ,Value=user.AccessToken});

        }

        public static void Logout()
        {
            HttpContext.Current.Session.Remove(LoginUserKey);
            var cookie = HttpContext.Current.Request.Cookies[AccessTokenKey];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-999);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}