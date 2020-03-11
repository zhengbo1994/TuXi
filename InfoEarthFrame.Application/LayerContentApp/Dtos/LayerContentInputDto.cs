using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.LayerContentApp.Dtos
{
	public class LayerContentInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 图层名称
		/// </summary>
		[StringLength(50)]
		public string LayerName { get; set; }
		/// <summary>
        /// 图层类型(点线面)
		/// </summary>
		[StringLength(36)]
		public string DataType { get; set; }
		/// <summary>
        /// 图层边界空间
		/// </summary>
		[StringLength(100)]
		public string LayerBBox { get; set; }
		/// <summary>
        /// 图层分类
		/// </summary>
		[StringLength(200)]
		public string LayerType { get; set; }
		/// <summary>
        /// 图层标签
		/// </summary>
		[StringLength(100)]
		public string LayerTag { get; set; }
		/// <summary>
        /// 图层描述
		/// </summary>
		[StringLength(100)]
		public string LayerDesc { get; set; }
		/// <summary>
        /// 图层业务表
		/// </summary>
		[StringLength(30)]
		public string LayerAttrTable { get; set; }
		/// <summary>
        /// 图层空间表
		/// </summary>
		[StringLength(30)]
		public string LayerSpatialTable { get; set; }
		/// <summary>
        /// 空间参考
		/// </summary>
		[StringLength(100)]
		public string LayerRefence { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDT { get; set; }
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
        [StringLength(1)]
        public string UploadStatus { get; set; }
        /// <summary>
        /// 拥有者
        /// </summary>
        [StringLength(50)]
        public string CreateBy { get; set; }
        /// <summary>
        /// 图层默认样式
        /// </summary>
        [MaxLength(36)]
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
	}
}

