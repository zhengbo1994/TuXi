using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InfoEarthFrame.Core
{
    /// <summary>
    /// 联系人信息
    /// </summary>
    [Table("TBL_MD_CONTACTPEOPLE")]
    public class Contactpeople : Entity<string>
    {
        /// <summary>
        /// 所属单位
        /// </summary>
        [MaxLength(50)]
        public string PoCId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [MaxLength(1)]
        public string Sex { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(100)]
        public string Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [MaxLength(50)]
        public string Fax { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [MaxLength(50)]
        public string City { get; set; }

        /// <summary>
        /// 行政区
        /// </summary>
        [MaxLength(50)]
        public string Province { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [MaxLength(50)]
        public string Country { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [MaxLength(10)]
        public string ZipCode { get; set; }

        /// <summary>
        /// 电子邮件地址
        /// </summary>
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// 个人网址
        /// </summary>
        [MaxLength(255)]
        public string WebSite { get; set; }

        /// <summary>
        /// 个人QQ号码
        /// </summary>
        [MaxLength(20)]
        public string QQ { get; set; }

        /// <summary>
        /// 个人MSN号码
        /// </summary>
        [MaxLength(100)]
        public string MSN { get; set; }
    }
}
