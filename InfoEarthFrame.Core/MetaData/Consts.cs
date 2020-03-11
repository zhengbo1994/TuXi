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
    /// 数据集限制信息表
    /// </summary>
    [Table("TBL_MD_CONSTS")]
    public class Consts : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 访问限制
        /// </summary>
        [MaxLength(50)]
        public string accessConsts { get; set; }

        /// <summary>
        /// 用途限制
        /// </summary>
        [MaxLength(50)]
        public string useLimit { get; set; }

        /// <summary>
        /// 安全等级（字典项）
        /// </summary>
        [MaxLength(50)]
        public string Classification { get; set; }

        /// <summary>
        /// 使用限制（字典项）
        /// </summary>
        [MaxLength(50)]
        public string useConsts { get; set; }
    }
}
