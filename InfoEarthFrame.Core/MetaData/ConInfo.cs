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
    /// 内容信息表
    /// </summary>
    [Table("TBL_MD_CONINFO")]
    public class ConInfo : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 栅格/影像内容描述
        /// </summary>
        [MaxLength(500)]
        public string cntRasterImage { get; set; }

        /// <summary>
        /// 属性结构描述文件
        /// </summary>
        [MaxLength(500)]
        public string cntAttrDescFile { get; set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        [MaxLength(500)]
        public string cntLayerName { get; set; }

        /// <summary>
        /// 要素类型名称
        /// </summary>
        [MaxLength(500)]
        public string cntFetTypes { get; set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        [MaxLength(500)]
        public string cntAttrTpyList { get; set; }

        

        
    }
}
