using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类DicDataType
    /// </summary>
    [Table("sdms_dictdatatype")]
    public class DicDataTypeEntity : Entity<string>
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
        /// 类型名称
        /// </summary>
        [MaxLength(50)]
        [Column("typename")]
        public string TypeName { get; set; }
        /// <summary>
        /// 类型描述
        /// </summary>
        [MaxLength(100)]
        [Column("typedesc")]
        public string TypeDesc { get; set; }
        /// <summary>
        /// 类型代码
        /// </summary>
        [MaxLength(36)]
        [Column("typecode")]
        public string TypeCode { get; set; }
        /// <summary>
        /// 类型排序
        /// </summary>
        [Column("typesort")]
        public int? TypeSort { get; set; }
        /// <summary>
        /// 父类型
        /// </summary>
        [MaxLength(36)]
        [Column("parentid")]
        public string ParentID { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [MaxLength(100)]
        [Column("keywords")]
        public string Keywords { get; set; }
    }
}