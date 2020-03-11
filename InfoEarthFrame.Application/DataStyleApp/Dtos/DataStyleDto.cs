using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Common.Style;

namespace InfoEarthFrame.Application.DataStyleApp.Dtos
{
	public class DataStyleDto : EntityDto
	{
        public string CurrentStyleId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 样式名称
		/// </summary>
		public string StyleName { get; set; }
        /// <summary>
        /// 样式类型
        /// </summary>
        public string StyleType { get; set; }
        /// <summary>
        /// 样式类型名称
        /// </summary>
        public string StyleTypeName { get; set; }
		/// <summary>
		/// 样式内容
		/// </summary>
		public string StyleContent { get; set; }
		/// <summary>
		/// 样式描述
		/// </summary>
		public string StyleDesc { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? CreateDT { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		public string CreateBy { get; set; }
        /// <summary>
        /// 样式类型
        /// </summary>
        public string StyleDataType { get; set; }
        /// <summary>
        /// 样式配置类型
        /// </summary>
        public string StyleConfigType { get; set; }
        /// <summary>
        /// 样式默认图层
        /// </summary>
        public string StyleDefaultLayer { get; set; }
        /// <summary>
        /// 样式渲染字段
        /// </summary>
        public string StyleRenderField { get; set; }
        /// <summary>
        /// 样式渲染颜色带
        /// </summary>
        public string StyleRenderColorBand { get; set; }
        /// <summary>
        /// 样式渲染预设规则
        /// </summary>
        public string StyleRenderRule { get; set; }

        ///// <summary>
        ///// 图层描述配置
        ///// </summary>
        //public StyledLayerDescriptor LayerDescriptor { get; set; }

        public bool LAY_CHECKED
        {
            get {
                return CurrentStyleId == Id;
            }
        }

	}
}

