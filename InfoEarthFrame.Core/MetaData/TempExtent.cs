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
    /// 时间范围信息
    /// </summary>
    [Table("TBL_EX_TEMPEXTENT")]
    public class TempExtent : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? begin { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end { get; set; }
    }
}
