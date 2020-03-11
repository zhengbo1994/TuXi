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
    /// 静态浏览图
    /// </summary>
    [Table("TBL_MD_BROWGRAPH")]
    public class BrowGraph : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 静态浏览图分类ID
        /// </summary>
        [MaxLength(50)]
        public string TypeID { get; set; }

        /// <summary>
        /// 静态浏览图文件名
        /// </summary>
        [MaxLength(255)]
        public string bgFileName { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        [MaxLength(500)]
        public string bgFileDesc { get; set; }

        /// <summary>
        /// 浏览图 
        /// </summary>
        public byte[] bgPicture { get; set; }
    }
}
