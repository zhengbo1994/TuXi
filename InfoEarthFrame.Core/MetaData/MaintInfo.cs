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
    /// 维护信息
    /// </summary>
    [Table("TBL_MD_MAINTINFO")]
    public class MaintInfo : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 更新范围说明
        /// </summary>
        [MaxLength(50)]
        public string upScpDesc { get; set; }

        /// <summary>
        /// 维护更新频率（字典项）
        /// </summary>
        [MaxLength(10)]
        public string maintFreq { get; set; }

        /// <summary>
        /// 负责人姓名
        /// </summary>
        [MaxLength(50)]
        public string rpIndName { get; set; }

        /// <summary>
        /// 负责人单位名称
        /// </summary>
        [MaxLength(50)]
        public string rpOrgName { get; set; }

        /// <summary>
        /// 职责
        /// </summary>
        [MaxLength(50)]
        public string rpPosName { get; set; }

        /// <summary>
        /// 职务（字典项）
        /// </summary>
        [MaxLength(10)]
        public string role { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [MaxLength(50)]
        public string cntFaxNum { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string cntDelPnt { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [MaxLength(50)]
        public string city { get; set; }

        /// <summary>
        /// 行政区
        /// </summary>
        [MaxLength(50)]
        public string adminArea { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [MaxLength(50)]
        public string country { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [MaxLength(10)]
        public string postCode { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        [MaxLength(255)]
        public string cntOnlineRes { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(30)]
        public string cntPhone { get; set; }

        /// <summary>
        /// 电子邮件地址
        /// </summary>
        [MaxLength(100)]
        public string eMailAddr { get; set; }
    }
}
