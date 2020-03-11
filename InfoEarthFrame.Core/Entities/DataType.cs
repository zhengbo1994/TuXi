using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类DataType
    /// </summary>
    [Table("sdms_datatype")]
    public class DataTypeEntity : Entity<string>
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
        /// 类别名称
        /// </summary>
        [MaxLength(50)]
        [Column("typename")]
        public string TypeName { get; set; }
        /// <summary>
        /// 类别描述
        /// </summary>
        [MaxLength(100)]
        [Column("typedesc")]
        public string TypeDesc { get; set; }
        /// <summary>
        /// 数据类型(地图类／图层类)
        /// </summary>
        [MaxLength(36)]
        [Column("dictcodeid")]
        public string DictCodeID { get; set; }
        /// <summary>
        /// 父类别
        /// </summary>
        [MaxLength(36)]
        [Column("parentid")]
        public string ParentID { get; set; }
    }
}