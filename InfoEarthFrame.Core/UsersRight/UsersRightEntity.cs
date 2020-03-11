using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{
    /// <summary>
    /// 用户组
    /// </summary>
   [Table("TBL_GROUP")]
    public class GroupEntity : Entity<string>
    {
       /// <summary>
       /// 用户组名称
       /// </summary>
       [MaxLength(2000)]
        public string Name { get; set; }      
    }

    /// <summary>
    /// 组成员
    /// </summary>
    [Table("TBL_GROUP_USER")]
   public class GroupUserEntity : Entity<string>
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        [MaxLength(200)]
        public string GroupId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [MaxLength(200)]
        public string UserId { get; set; }
    }

    /// <summary>
    /// 组权限
    /// </summary>
    [Table("TBL_GROUP_RIGHT")]
    public class GroupRightEntity : Entity<string>
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        [MaxLength(200)]
        public string GroupId { get; set; }
        /// <summary>
        /// 图层ID
        /// </summary>
        [MaxLength(200)]
        public string LayerId { get; set; }
        /// <summary>
        /// 是否下载（1是，0否）
        /// </summary>
        public int IsDownload { get; set; }
        /// <summary>
        /// 是否浏览（1是，0否）
        /// </summary>
        public int IsBrowse { get; set; }

        public bool IsParent { get; set; }
    } 

}


