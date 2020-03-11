using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.MapReleationApp.Dtos
{
	[AutoMapFrom(typeof(MapReleationEntity))]
	public class MapReleationOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
        /// 地图ID
		/// </summary>
		public string MapID { get; set; }
		/// <summary>
        /// 图层目录ID
		/// </summary>
		public string DataConfigID { get; set; }
		/// <summary>
        /// 图层样式ID
		/// </summary>
		public string DataStyleID { get; set; }
		/// <summary>
        /// 图层排序
		/// </summary>
		public int? DataSort { get; set; }
		/// <summary>
        /// 配置日期
		/// </summary>
		public DateTime? ConfigDT { get; set; }
		/// <summary>
        /// 修改日期
		/// </summary>
		public DateTime? ModifyDT { get; set; }

	}
}

