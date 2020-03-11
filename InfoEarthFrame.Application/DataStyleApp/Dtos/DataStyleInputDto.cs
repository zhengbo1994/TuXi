using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Common.Style;

namespace InfoEarthFrame.Application.DataStyleApp.Dtos
{
    public class QueryDataStyleInputParamDto
    {
        public string StyleName { get; set; }
        public string StyleType { get; set; }
        public string Createby { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class UpdateDefaultStyleDto
    {
        public string layerID { get; set; }

        public string styleID { get; set; }

        public string user { get; set; }
    }
	public class DataStyleInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 样式名称
		/// </summary>
		[StringLength(50)]
		public string StyleName { get; set; }
        /// <summary>
        /// 样式类型
        /// </summary>
        [StringLength(36)]
        public string StyleType { get; set; }
		/// <summary>
        /// 样式内容
		/// </summary>
		//[StringLength(40000)]
		public string StyleContent { get; set; }
		/// <summary>
        /// 样式描述
		/// </summary>
		[StringLength(100)]
		public string StyleDesc { get; set; }
		/// <summary>
        /// 创建时间
		/// </summary>
		public DateTime? CreateDT { get; set; }
		/// <summary>
        /// 创建人
		/// </summary>
		[StringLength(36)]
		public string CreateBy { get; set; }
        /// <summary>
        /// 样式类型
        /// </summary>
        [StringLength(36)]
        public string StyleDataType { get; set; }

        /// <summary>
        /// 样式配置类型
        /// </summary>
        [StringLength(36)]
        public string StyleConfigType { get; set; }
        /// <summary>
        /// 样式默认图层
        /// </summary>
        [StringLength(36)]
        public string StyleDefaultLayer { get; set; }
        /// <summary>
        /// 样式渲染字段
        /// </summary>
        [StringLength(50)]
        public string StyleRenderField { get; set; }

        public string StyleRenderFieldName { get; set; }

        /// <summary>
        /// 样式渲染颜色带
        /// </summary>
        [StringLength(100)]
        public string StyleRenderColorBand { get; set; }
        /// <summary>
        /// 样式渲染预设规则
        /// </summary>
        [StringLength(500)]
        public string StyleRenderRule { get; set; }

        public string DefaultStyleId { get; set; }
        ///// <summary>
        ///// 图层描述配置
        ///// </summary>
        //public StyledLayerDescriptor LayerDescriptor { get; set; }

        public InfoEarthFrame.Application.DataStyleApp.DataStyleAppService.StyleInfo StyleInfo { get; set; }

        public List<InfoEarthFrame.Application.DataStyleApp.DataStyleAppService.RuleData> RuleDatas { get; set; }
    }
}

