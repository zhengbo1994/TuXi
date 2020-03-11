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
    /// 数据集格式
    /// </summary>
    [Table("TBL_MD_FORMAT")]
    public class Format : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 格式名称
        /// </summary>
        [MaxLength(50)]
        public string fomatName { get; set; }

        /// <summary>
        /// 格式版本
        /// </summary>
        [MaxLength(50)]
        public string formatVer { get; set; }
    }
}
