using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类LayerContent
    /// </summary>
    [Table("sdms_layer")]
    public class LayerContentEntity : Entity<string>
    {
        [Column("id")]
        [MaxLength(36)]
        public override string Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }
        /// <summary>
        /// 图层名称
        /// </summary>
        [MaxLength(50)]
        [Column("layername")]
        public string LayerName { get; set; }
        /// <summary>
        /// 图层类型(点线面)
        /// </summary>
        [MaxLength(36)]
        [Column("datatype")]
        public string DataType { get; set; }
        /// <summary>
        /// 图层边界空间
        /// </summary>
        [MaxLength(100)]
        [Column("layerbbox")]
        public string LayerBBox { get; set; }
        /// <summary>
        /// 图层分类
        /// </summary>
        [MaxLength(200)]
        [Column("layertype")]
        public string LayerType { get; set; }
        /// <summary>
        /// 图层标签
        /// </summary>
        [MaxLength(100)]
        [Column("layertag")]
        public string LayerTag { get; set; }
        /// <summary>
        /// 图层描述
        /// </summary>
        [MaxLength(100)]
        [Column("layerdesc")]
        public string LayerDesc { get; set; }
        /// <summary>
        /// 图层业务表
        /// </summary>
        [MaxLength(30)]
        [Column("layerattrtable")]
        public string LayerAttrTable { get; set; }
        /// <summary>
        /// 图层空间表
        /// </summary>
        [MaxLength(30)]
        [Column("layerspatialtable")]
        public string LayerSpatialTable { get; set; }
        /// <summary>
        /// 空间参考
        /// </summary>
        [MaxLength(100)]
        [Column("layerrefence")]
        public string LayerRefence { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdt")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// north(北)
        /// </summary>
        [Column("maxy")]
        public decimal? MaxY { get; set; }
        /// <summary>
        /// south(南)
        /// </summary>
        [Column("miny")]
        public decimal? MinY { get; set; }
        /// <summary>
        /// west(西)
        /// </summary>
        [Column("minx")]
        public decimal? MinX { get; set; }
        /// <summary>
        /// east(东)
        /// </summary>
        [Column("maxx")]
        public decimal? MaxX { get; set; }
        /// <summary>
        /// 上传状态
        /// </summary>
        [MaxLength(1)]
        [Column("uploadstatus")]
        public string UploadStatus { get; set; }
        /// <summary>
        /// 拥有者
        /// </summary>
        [MaxLength(50)]
        [Column("createby")]
        public string CreateBy { get; set; }
        /// <summary>
        /// 图层默认样式
        /// </summary>
        [MaxLength(36)]
        [Column("layerdefaultstyle")]
        public string LayerDefaultStyle { get; set; }


        /// <summary>
        /// 上传图层样式（1-矢量图层；2-影像图层）
        /// </summary>
        [MaxLength(1)]
        [Column("uploadfiletype")]
        public string UploadFileType { get; set; }

        /// <summary>
        /// 上传文件名称
        /// </summary>
        [MaxLength(200)]
        [Column("uploadfilename")]
        public string UploadFileName { get; set; }

        [Column("mainid")]
        public string MainId { get; set; }

    }
}

