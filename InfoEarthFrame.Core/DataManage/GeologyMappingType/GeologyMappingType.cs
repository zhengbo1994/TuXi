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
    /// 中国地质环境图系 分类
    /// </summary>
    [Table("TBL_GEOLOGYMAPPINGTYPE")]
    public class GeologyMappingType : Entity<string>
    {
        /// <summary>
        /// 上级ID
        /// </summary>
        [MaxLength(50)]
        public string ParentID { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [MaxLength(2000)]
        public string Paths { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(50)]
        public string ClassName { get; set; }
    }


    //public class GeologyMappingTypeEx : GeologyMappingType
    //{
    //    /// <summary>
    //    /// 是否下载（1是，0否）
    //    /// </summary>
    //    public int IsDownload { get; set; }
    //    /// <summary>
    //    /// 是否浏览（1是，0否）
    //    /// </summary>
    //    public int IsBrowse { get; set; }
    //}
}
