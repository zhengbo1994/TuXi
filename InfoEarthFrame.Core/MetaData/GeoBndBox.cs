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
    /// 地理坐标范围信息
    /// </summary>
    [Table("TBL_EX_GEOBNDBOX")]
    public class GeoBndBox : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 西经
        /// </summary>
        public decimal? westBL { get; set; }

        /// <summary>
        /// 东经
        /// </summary>
        public decimal? eastBL { get; set; }

        /// <summary>
        /// 南纬
        /// </summary>
        public decimal? northBL { get; set; }

        /// <summary>
        /// 北经
        /// </summary>
        public decimal? southBL { get; set; }
    }
}
