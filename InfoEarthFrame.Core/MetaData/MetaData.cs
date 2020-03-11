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
    /// 元数据信息表
    /// </summary>
    [Table("TBL_MD_METADATA")]
    public class MetaData : Entity<string>
    {
        /// <summary>
        /// 所属MainFile
        /// </summary>
        [MaxLength(50)]
        public string MainID { get; set; }

        /// <summary>
        /// 所属File
        /// </summary>
        [MaxLength(50)]
        public string FileID { get; set; }

        /// <summary>
        /// 元数据名称
        /// </summary>
        [MaxLength(50)]
        public string mdTitle { get; set; }
        /// <summary>
        /// 元数据创建日期
        /// </summary>
        public DateTime? mdDataSt { get; set; }
        /// <summary>
        /// 字符集（字典项）
        /// </summary>
         [MaxLength(5)]
        public string mdChar { get; set; }
         /// <summary>
         /// 元数据标准名称
         /// </summary>
         [MaxLength(100)]
         public string mdStanName { get; set; }
         /// <summary>
         /// 元数据标准版本
         /// </summary>
         [MaxLength(10)]
         public string mdStanVer { get; set; }
        /// <summary>
        /// 语种
        /// </summary>
        [MaxLength(50)]
         public string mdLang { get; set; }
    }
}
