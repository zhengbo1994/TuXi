using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类TagReleation
    /// </summary>
    [Table("sdms_tag_releation")]
    public class TagReleationEntity : Entity<string>
    {
        /// <summary>
        /// 标签关系ID
        /// </summary>
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
        /// 标签ID
        /// </summary>
        [MaxLength(36)]
        [Column("datatagid")]
        public string DataTagID { get; set; }
        /// <summary>
        /// 地图或图层ID
        /// </summary>
        [MaxLength(36)]
        [Column("mapid")]
        public string MapID { get; set; }

    }
}