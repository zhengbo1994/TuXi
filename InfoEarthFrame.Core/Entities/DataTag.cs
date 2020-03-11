using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类DataTag
    /// </summary>
    [Table("sdms_datatag")]
    public class DataTagEntity : Entity<string>
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
        /// 标签名称
        /// </summary>
        [MaxLength(50)]
        [Column("tagname")]
        public string TagName { get; set; }
        /// <summary>
        /// 标签描述
        /// </summary>
        [MaxLength(100)]
        [Column("tagdesc")]
        public string TagDesc { get; set; }
        /// <summary>
        /// 数据类型(地图类／图层类)
        /// </summary>
        [MaxLength(36)]
        [Column("dictcodeid")]
        public string DictCodeID { get; set; }
    }
}