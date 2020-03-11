using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerContentApp.Dtos
{
	[AutoMapFrom(typeof(LayerContentEntity))]
	public class LayerContentOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
        /// 图层名称
		/// </summary>
		public string LayerName { get; set; }
		/// <summary>
        /// 图层类型(点线面)
		/// </summary>
		public string DataType { get; set; }
		/// <summary>
        /// 图层边界空间
		/// </summary>
		public string LayerBBox { get; set; }
		/// <summary>
        /// 图层分类
		/// </summary>
		public string LayerType { get; set; }
		/// <summary>
        /// 图层标签
		/// </summary>
		public string LayerTag { get; set; }
		/// <summary>
        /// 图层描述
		/// </summary>
		public string LayerDesc { get; set; }
		/// <summary>
        /// 图层业务表
		/// </summary>
		public string LayerAttrTable { get; set; }
		/// <summary>
        /// 图层空间表
		/// </summary>
		public string LayerSpatialTable { get; set; }
		/// <summary>
        /// 空间参考
		/// </summary>
		public string LayerRefence { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDT { get; set; }
        /// <summary>
		/// 图层样式ID
		/// </summary>
		public string DataStyleID { get; set; }
        /// <summary>
        /// 图层样式名称
        /// </summary>
        public string DataStyleName { get; set; }
        /// <summary>
        /// 图层排序
        /// </summary>
        public int? DataSort { get; set; }
        /// <summary>
        /// north(北)
        /// </summary>
        public decimal? MaxY { get; set; }
        /// <summary>
        /// south(南)
        /// </summary>
        public decimal? MinY { get; set; }
        /// <summary>
        /// west(西)
        /// </summary>
        public decimal? MinX { get; set; }
        /// <summary>
        /// east(东)
        /// </summary>
        public decimal? MaxX { get; set; }
        /// <summary>
        /// 上传状态
        /// </summary>
        public string UploadStatus { get; set; }
        /// <summary>
        /// 拥有者
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 图层默认样式
        /// </summary>
        public string LayerDefaultStyle { get; set; }

        /// <summary>
        /// 上传图层样式（1-矢量图层；2-影像图层）
        /// </summary>
        [MaxLength(1)]
        public string UploadFileType { get; set; }

        /// <summary>
        /// 上传文件名称
        /// </summary>
        [MaxLength(200)]
        public string UploadFileName { get; set; }

        public string CreateUser { get; set; }

     
    }
}

