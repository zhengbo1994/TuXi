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
    /// 数据管理（主表）
    /// </summary>
    [Table("TBL_DATAMAIN")]
    public class DataMain : Entity<string>
    {
        /// <summary>
        /// 所属类型
        /// </summary>
         [MaxLength(50)]
        public string MappingTypeID { get; set; }

         /// <summary>
         /// 比例尺分母
         /// </summary>
         public int? Scale { get; set; }


        /// <summary>
        /// ZIP文件名
        /// </summary>
        [MaxLength(100)]
        public string ZipFileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [MaxLength(500)]
        public string FilePath { get; set; }

        /// <summary>
        /// 主要SHP文件名
        /// </summary>
        [MaxLength(100)]
        public string MainShpFileName { get; set; }

        /// <summary>
        /// 版本 1 验收版 和 2 最终版
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int VersionNo { get; set; }


        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? StorageTime { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? ReleaseTime { get; set; }

        /// <summary>
        /// 入库人编号
        /// </summary>
        [MaxLength(50)]
        public string CreaterID { get; set; }

        /// <summary>
        /// 入库人
        /// </summary>
         [MaxLength(30)]
        public string Creater { get; set; }

         /// <summary>
         /// 图片路径
         /// </summary>
         [MaxLength(200)]
         public string ImagePath { get; set; }

         public string Name { get; set; }

        /// <summary>
         /// 状态1.处理中 2.成功 3.失败
        /// </summary>
         public int? Status { get; set; }


         public string Message { get; set; }
    }
}
