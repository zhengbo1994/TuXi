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
    /// 关键词
    /// </summary>
    [Table("TBL_EX_KEYWORDS")]
    public class KeyWords : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [MaxLength(10)]
        public string keyTyp { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [MaxLength(200)]
        public string keyword { get; set; }
    }
}
