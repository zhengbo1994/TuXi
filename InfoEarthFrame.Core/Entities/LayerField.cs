using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类LayerField
    /// </summary>
    [Table("sdms_layerfield")]
    public class LayerFieldEntity : Entity<string>
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
        /// 图层目录ID
        /// </summary>
        [MaxLength(36)]
        [Column("layerid")]
        public string LayerID { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [MaxLength(50)]
        [Column("attributename")]
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性描述
        /// </summary>
        [MaxLength(100)]
        [Column("attributedesc")]
        public string AttributeDesc { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        [MaxLength(36)]
        [Column("attributetype")]
        public string AttributeType { get; set; }
        /// <summary>
        /// 属性长度
        /// </summary>
        [MaxLength(5)]
        [Column("attributelength")]
        public string AttributeLength { get; set; }
        /// <summary>
        /// 属性小数位
        /// </summary>
        [MaxLength(5)]
        [Column("attributeprecision")]
        public string AttributePrecision { get; set; }
        /// <summary>
        /// 输入控制
        /// </summary>
        [MaxLength(50)]
        [Column("attributeinputctrl")]
        public string AttributeInputCtrl { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        [MaxLength(10)]
        [Column("attributeinputmax")]
        public string AttributeInputMax { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        [MaxLength(10)]
        [Column("attributeinputmin")]
        public string AttributeInputMin { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        [MaxLength(30)]
        [Column("attributedefault")]
        public string AttributeDefault { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        [MaxLength(1)]
        [Column("attributeisnull")]
        public string AttributeIsNull { get; set; }
        /// <summary>
        /// 输入格式
        /// </summary>
        [MaxLength(50)]
        [Column("attributeinputformat")]
        public string AttributeInputFormat { get; set; }
        /// <summary>
        /// 备注
		/// </summary>
		[MaxLength(200)]
        [Column("remark")]
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdt")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(50)]
        [Column("attributeunit")]
        public string AttributeUnit { get; set; }
        /// <summary>
        /// 数据分类
        /// </summary>
        [MaxLength(50)]
        [Column("attributedatatype")]
        public string AttributeDataType { get; set; }
        /// <summary>
        /// 文字符连接
        /// </summary>
        [MaxLength(50)]
        [Column("attributevaluelink")]
        public string AttributeValueLink { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        [MaxLength(50)]
        [Column("attributedatasource")]
        public string AttributeDataSource { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        [MaxLength(50)]
        [Column("attributecalcomp")]
        public string AttributeCalComp { get; set; }
        /// <summary>
        /// 属性排序
        /// </summary>
        [Column("attributesort")]
        public int? AttributeSort { get; set; }

    }
}