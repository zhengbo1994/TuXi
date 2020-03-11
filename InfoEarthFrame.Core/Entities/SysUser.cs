using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类DataStyle
    /// </summary>
    [Table("sdms_user")]
    public class SystemUserEntity : Entity<string>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        [MaxLength(50)]
        public string UserCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [MaxLength(5)]
        public string UserSex { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [MaxLength(50)]
        public string Password { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(20)]
        public string TelPhone { get; set; }
        /// <summary>
        /// 联系手机
        /// </summary>
        [MaxLength(20)]
        public string Phone { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [MaxLength(100)]
        public string Department { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        [MaxLength(100)]
        public string Position { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(4000)]
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDT { get; set; }
    }
}
