using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类Map
    /// </summary>
    [Table("sdms_map")]
    public class MapEntity : Entity<string>
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
        /// 地图名称
        /// </summary>
        [MaxLength(50)]
        [Column("mapname")]
        public string MapName { get; set; }

        /// <summary>
        /// 地图英文名称
        /// </summary>
        [MaxLength(50)]
        [Column("mapenname")]
        public string MapEnName { get; set; }

        /// <summary>
        /// 边界范围
        /// </summary>
        [MaxLength(100)]
        [Column("mapbbox")]
        public string MapBBox { get; set; }
        /// <summary>
        /// 地图发布地址
        /// </summary>
        [MaxLength(200)]
        [Column("mappublishaddress")]
        public string MapPublishAddress { get; set; }
        /// <summary>
        /// 地图状态
        /// </summary>
        [MaxLength(36)]
        [Column("mapstatus")]
        public string MapStatus { get; set; }
        /// <summary>
        /// 地图描述
        /// </summary>
        [MaxLength(100)]
        [Column("mapdesc")]
        public string MapDesc { get; set; }
        /// <summary>
        /// 地图分类
        /// </summary>
        [MaxLength(50)]
        [Column("maptype")]
        public string MapType { get; set; }
        /// <summary>
        /// 地图标签
        /// </summary>
        [MaxLength(200)]
        [Column("maptag")]
        public string MapTag { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [Column("publishdt")]
        public DateTime? PublishDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("sortcode")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("enabledmark")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deletemark")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        [Column("createuserid")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        [Column("createusername")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("createdt")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        [Column("modifyuserid")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        [Column("modifyusername")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("modifydate")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 比例尺
        /// </summary>
        [MaxLength(36)]
        [Column("mapscale")]
        public string MapScale { get; set; }
        /// <summary>
        /// 空间参考
        /// </summary>
        [MaxLength(100)]
        [Column("spatialrefence")]
        public string SpatialRefence { get; set; }
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
        /// 地图图例
        /// </summary>
        [MaxLength(200)]
        [Column("maplegend")]
        public string MapLegend { get; set; }
    }
}