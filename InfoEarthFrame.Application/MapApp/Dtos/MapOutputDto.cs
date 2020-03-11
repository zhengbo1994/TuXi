using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.MapApp.Dtos
{
	[AutoMapFrom(typeof(MapEntity))]
	public class MapOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
        /// 地图名称
		/// </summary>
		public string MapName { get; set; }


        /// <summary>
        /// 地图英文名称
        /// </summary>
        public string MapEnName { get; set; }

		/// <summary>
        /// 边界范围
		/// </summary>
		public string MapBBox { get; set; }
		/// <summary>
        /// 地图发布地址
		/// </summary>
		public string MapPublishAddress { get; set; }
		/// <summary>
        /// 地图状态
		/// </summary>
		public string MapStatus { get; set; }
		/// <summary>
        /// 地图描述
		/// </summary>
		public string MapDesc { get; set; }
		/// <summary>
        /// 地图分类
		/// </summary>
		public string MapType { get; set; }
		/// <summary>
        /// 地图标签
		/// </summary>
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
		public string CreateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CreateUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ModifyUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ModifyUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 比例尺
        /// </summary>
        public string MapScale { get; set; }
        /// <summary>
        /// 比例尺名称
        /// </summary>
        public string MapScaleName { get; set; }
        /// <summary>
        /// 空间参考
        /// </summary>
        public string SpatialRefence { get; set; }
        /// <summary>
        /// 空间参考名称
        /// </summary>
        public string SpatialRefenceName { get; set; }
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
        public string MapLegend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MaxYName { get; set; }
        /// <summary>
        /// south(南)
        /// </summary>
        public string MinYName { get; set; }
        /// <summary>
        /// west(西)
        /// </summary>
        public string MinXName { get; set; }
        /// <summary>
        /// east(东)
        /// </summary>
        public string MaxXName { get; set; }

	}
}

