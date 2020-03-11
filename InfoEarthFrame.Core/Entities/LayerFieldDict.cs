using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类LayerFieldDict
    /// </summary>
    [Table("sdms_layerfielddict")]
    public class LayerFieldDictEntity : Entity<string>
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
        /// 属性ID
        /// </summary>
        [MaxLength(36)]
        [Column("attributeid")]
        public string AttributeID { get; set; }
        /// <summary>
        /// 属性字典值
        /// </summary>
        [MaxLength(50)]
        [Column("fielddictname")]
        public string FieldDictName { get; set; }
        /// <summary>
        /// 属性字典描述
        /// </summary>
        [MaxLength(100)]
        [Column("fielddictdesc")]
        public string FieldDictDesc { get; set; }

    }
}

