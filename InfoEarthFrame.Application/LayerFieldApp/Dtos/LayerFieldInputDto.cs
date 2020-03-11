using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.LayerFieldApp.Dtos
{
	public class LayerFieldInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 图层目录ID
		/// </summary>
		[StringLength(36)]
		public string LayerID { get; set; }
		/// <summary>
        /// 属性名称
		/// </summary>
		[StringLength(50)]
		public string AttributeName { get; set; }
		/// <summary>
        /// 属性描述
		/// </summary>
		[StringLength(100)]
		public string AttributeDesc { get; set; }
		/// <summary>
        /// 属性类型
		/// </summary>
		[StringLength(36)]
		public string AttributeType { get; set; }
		/// <summary>
        /// 属性长度
		/// </summary>
		[StringLength(5)]
		public string AttributeLength { get; set; }
		/// <summary>
        /// 属性小数位
		/// </summary>
		[StringLength(5)]
		public string AttributePrecision { get; set; }
		/// <summary>
        /// 输入控制
		/// </summary>
		[StringLength(50)]
		public string AttributeInputCtrl { get; set; }
		/// <summary>
        /// 最大值
		/// </summary>
		[StringLength(10)]
		public string AttributeInputMax { get; set; }
		/// <summary>
        /// 最小值
		/// </summary>
		[StringLength(10)]
		public string AttributeInputMin { get; set; }
		/// <summary>
        /// 默认值
		/// </summary>
		[StringLength(30)]
		public string AttributeDefault { get; set; }
		/// <summary>
        /// 是否为空
		/// </summary>
		[StringLength(1)]
		public string AttributeIsNull { get; set; }
		/// <summary>
        /// 输入格式
		/// </summary>
		[StringLength(50)]
		public string AttributeInputFormat { get; set; }
		/// <summary>
        /// 备注
		/// </summary>
		[StringLength(200)]
		public string Remark { get; set; }
		/// <summary>
        /// 创建时间
		/// </summary>
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(50)]
        public string AttributeUnit { get; set; }
        /// <summary>
        /// 数据分类
        /// </summary>
        [StringLength(50)]
        public string AttributeDataType { get; set; }
        /// <summary>
        /// 文字符连接
        /// </summary>
        [StringLength(50)]
        public string AttributeValueLink { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        [StringLength(50)]
        public string AttributeDataSource { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        [StringLength(50)]
        public string AttributeCalComp { get; set; }
        /// <summary>
        /// 属性排序
        /// </summary>
        public int? AttributeSort { get; set; }

	}
}

