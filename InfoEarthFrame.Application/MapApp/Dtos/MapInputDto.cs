using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.MapApp.Dtos
{
	public class MapInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 地图名称
		/// </summary>
		[StringLength(50)]
		public string MapName { get; set; }
		/// <summary>
        /// 边界范围
		/// </summary>
		[StringLength(100)]
		public string MapBBox { get; set; }
		/// <summary>
        /// 地图发布地址
		/// </summary>
		[StringLength(200)]
		public string MapPublishAddress { get; set; }
		/// <summary>
        /// 地图状态
		/// </summary>
		[StringLength(36)]
		public string MapStatus { get; set; }
		/// <summary>
        /// 地图描述
		/// </summary>
		[StringLength(100)]
		public string MapDesc { get; set; }
		/// <summary>
        /// 地图分类
		/// </summary>
		[StringLength(50)]
		public string MapType { get; set; }
		/// <summary>
        /// 地图标签
		/// </summary>
		[StringLength(200)]
		public string MapTag { get; set; }
		/// <summary>
        /// 发布时间
		/// </summary>
		public DateTime? PublishDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? SortCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? EnabledMark { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? DeleteMark { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string CreateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string CreateUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string ModifyUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string ModifyUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 比例尺
        /// </summary>
        [StringLength(36)]
        public string MapScale { get; set; }
        /// <summary>
        /// 空间参考
        /// </summary>
        [StringLength(100)]
        public string SpatialRefence { get; set; }
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
        /// 地图图例
        /// </summary>
        [StringLength(200)]
        public string MapLegend { get; set; } 

	}
}

