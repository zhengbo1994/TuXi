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
    /// 空间参照信息信息
    /// </summary>
    [Table("TBL_MD_REFSYSINFO")]
    public class RefSysInfo : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(20)]
        public string refSysName { get; set; }

        /// <summary>
        /// 坐标参照系名称
        /// </summary>
        [MaxLength(20)]
        public string coodRSID { get; set; }

        /// <summary>
        /// 坐标类型
        /// </summary>
        [MaxLength(20)]
        public string coodType { get; set; }

        /// <summary>
        /// 坐标系名称
        /// </summary>
        [MaxLength(50)]
        public string coodSID { get; set; }

        /// <summary>
        /// 投影参数
        /// </summary>
        [MaxLength(50)]
        public string parameter { get; set; }

        /// <summary>
        /// 垂直坐标参照系名称
        /// </summary>
        [MaxLength(50)]
        public string verRSID { get; set; }
    }
}
