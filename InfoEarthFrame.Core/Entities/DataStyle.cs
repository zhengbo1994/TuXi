using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类DataStyle
    /// </summary>
    [Table("sdms_datastyle")]
    public class DataStyleEntity : Entity<string>
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
        /// 样式名称
        /// </summary>
        [MaxLength(50)]
        [Column("stylename")]
        public string StyleName { get; set; }
        /// <summary>
        /// 样式分类
        /// </summary>
        [MaxLength(36)]
        [Column("styletype")]
        public string StyleType { get; set; }
        /// <summary>
        /// 样式内容
        /// </summary>
        [MaxLength(int.MaxValue)]
        [Column("stylecontent")]
        public string StyleContent { get; set; }
        /// <summary>
        /// 样式描述
        /// </summary>
        [MaxLength(100)]
        [Column("styledesc")]
        public string StyleDesc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdt")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(36)]
        [Column("createby")]
        public string CreateBy { get; set; }
        /// <summary>
        /// 样式类型
        /// </summary>
        [MaxLength(36)]
        [Column("styledatatype")]
        public string StyleDataType { get; set; }

        /// <summary>
        /// 样式配置类型
        /// </summary>
        [MaxLength(36)]
        [Column("styleconfigtype")]
        public string StyleConfigType { get; set; }

        /// <summary>
        /// 样式默认图层
        /// </summary>
        [MaxLength(36)]
        [Column("styledefaultlayer")]
        public string StyleDefaultLayer { get; set; }
        /// <summary>
        /// 样式渲染字段
        /// </summary>
        [MaxLength(50)]
        [Column("stylerenderfield")]
        public string StyleRenderField { get; set; }
        /// <summary>
        /// 样式渲染颜色带
        /// </summary>
        [MaxLength(100)]
        [Column("stylerendercolorband")]
        public string StyleRenderColorBand { get; set; }
        /// <summary>
        /// 样式渲染预设规则
        /// </summary>
        [MaxLength(500)]
        [Column("stylerenderrule")]
        public string StyleRenderRule { get; set; }

    }
}

