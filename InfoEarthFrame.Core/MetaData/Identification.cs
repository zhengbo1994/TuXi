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
    /// 标识信息表
    /// </summary>
    [Table("TBL_MD_IDENTIFICATION")]
    public class Identification : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 字符集（字典项）
        /// </summary>
        [MaxLength(20)]
        public string mdChar { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [MaxLength(500)]
        public string idAbs { get; set; }
        /// <summary>
        /// 目的
        /// </summary>
        [MaxLength(200)]
        public string idPurp { get; set; }
        /// <summary>
        /// 状况（字典项）
        /// </summary>
        [MaxLength(20)]
        public string idStatus { get; set; }
        /// <summary>
        /// 音像轨道标识
        /// </summary>
        [MaxLength(50)]
        public string imageID { get; set; }
        /// <summary>
        /// 语种
        /// </summary>
        [MaxLength(50)]
        public string dataLangCode { get; set; }
        /// <summary>
        /// 数据表示方式（字典项）
        /// </summary>
        [MaxLength(20)]
        public string dataRpType { get; set; }
        /// <summary>
        /// 比例尺
        /// </summary>
        [MaxLength(50)]
        public string dataScale { get; set; }
        /// <summary>
        /// 专题类型（字典项）
        /// </summary>
       [MaxLength(50)]
        public string tpCat { get; set; }
       /// <summary>
       /// 地理标识符
       /// </summary>
       [MaxLength(50)]
       public string geoId { get; set; }
    }
}
