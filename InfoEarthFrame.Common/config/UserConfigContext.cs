using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.Common
{
    public class UserConfigContext
    {
        private static readonly string CurrentLangKey = ConfigContext.Current.DefaultConfig["CurrentLangKey"];

        /// <summary>
        /// 客户端当前所使用的语言
        /// </summary>
        public static string CurrentLang
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[CurrentLangKey];
                if (cookie == null)
                {
                    return "zh-cn";
                }

                return cookie.Value;
            }
            set
            { 
                var cookie=new HttpCookie(CurrentLangKey);
                cookie.Value=value;
                cookie.Expires = DateTime.Now.AddDays(999);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}
