using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class GroupOutput
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户组名称
        /// </summary>
        public string Name { get; set; }  

        /// <summary>
        /// 用户信息
        /// </summary>
        public List<GroupWithUserEntity> UserInfo { get; set; }

        /// <summary>
        /// 授权信息
        /// </summary>
        public List<GroupWithRightEntity> RightInfo { get; set; } 
    }
}
