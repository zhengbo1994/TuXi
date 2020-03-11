using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    [Serializable]
   public  class LoginUser
    {
        /// <summary>
        /// 登陆用户名
        /// </summary>
        public string Name { get; set; }

       /// <summary>
       /// 密码
       /// </summary>
        public string Password { get; set; }

        public string AccessToken { get; set; }
    }
}
