using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类MapReleation
    /// </summary>
    [Table("sdms_map_releation")]
    public class MapReleationEntity : Entity<string>
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
        /// 地图ID
        /// </summary>
        [MaxLength(36)]
        [Column("mapid")]
        public string MapID { get; set; }
        /// <summary>
        /// 图层目录ID
        /// </summary>
        [MaxLength(36)]
        [Column("dataconfigid")]
        public string DataConfigID { get; set; }
        /// <summary>
        /// 图层样式ID
        /// </summary>
        [MaxLength(36)]
        [Column("datastyleid")]
        public string DataStyleID { get; set; }
        /// <summary>
        /// 图层排序
        /// </summary>
        [Column("datasort")]
        public int? DataSort { get; set; }
        /// <summary>
        /// 配置日期
        /// </summary>
        [Column("configdt")]
        public DateTime? ConfigDT { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Column("modifydt")]
        public DateTime? ModifyDT { get; set; }
    }
}