using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{

    /// <summary>
    /// 灾害体类型字典
    /// </summary>
    [Table("DIC_HAZARDSTYPE")]
    public class HazardsTypeEntity : Entity<string>
    {
        /// <summary>
        /// 灾害体类型
        /// </summary>
        [MaxLength(100)]
        public string HAZARDSTYPE { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [MaxLength(100)]
        public string TABNAME { get; set; }

        /// <summary>
        /// CGHmdb系统灾害体类型ID
        /// </summary>
        [MaxLength(100)]
        public string CGHID { get; set; }
    }
}
