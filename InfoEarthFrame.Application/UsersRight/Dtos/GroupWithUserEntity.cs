using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class GroupWithUserEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
         
        /// <summary>
        /// 用户组id
        /// </summary>
        public string GroupId { get; set; } 
                /// <summary>
        /// 用户组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户组名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户单位
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 用户登陆名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// checkedUser
        /// </summary>
        public bool checkedUser { get; set; }
    }
}
