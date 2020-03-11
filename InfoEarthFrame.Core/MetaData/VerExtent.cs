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
    /// 垂直范围信息
    /// </summary>
    [Table("TBL_EX_VEREXTENT")]
    public class VerExtent : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 最大垂向坐标
        /// </summary>
        public decimal? vertMaxVal { get; set; }

        /// <summary>
        /// 垂向计量单位
        /// </summary>
        [MaxLength(10)]
        public string vertUoM { get; set; }

        /// <summary>
        /// 垂向基准名称代码
        /// </summary>
        [MaxLength(10)]
        public string vertDatum { get; set; }

        /// <summary>
        /// 最小垂向坐标
        /// </summary>
        public decimal? vertMinVal { get; set; }

        
    }
}
