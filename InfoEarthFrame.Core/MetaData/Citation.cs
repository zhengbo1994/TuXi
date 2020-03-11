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
    /// 引用资料信息
    /// </summary>
    [Table("TBL_CI_CITATION")]
    public class Citation : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        public string resTitle { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [MaxLength(100)]
        public string resEd { get; set; }

        /// <summary>
        /// 版本日期
        /// </summary>
        public DateTime? resEdDate { get; set; }

        /// <summary>
        /// 国际标准书号
        /// </summary>
        [MaxLength(50)]
        public string Isbn { get; set; }

        /// <summary>
        /// 国际标准系列号
        /// </summary>
        [MaxLength(50)]
        public string Issn { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? rsDate { get; set; }

        /// <summary>
        /// 引用资料的负责单位
        /// </summary>
        [MaxLength(200)]
        public string citRespParty { get; set; }

        /// <summary>
        /// 表达形式（字典项）
        /// </summary>
        [MaxLength(5)]
        public string presForm { get; set; }

        
    }
}
