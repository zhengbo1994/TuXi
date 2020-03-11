using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.MapReleationApp.Dtos
{
	public class MapReleationInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 地图ID
		/// </summary>
		[StringLength(36)]
		public string MapID { get; set; }
		/// <summary>
        /// 图层目录ID
		/// </summary>
		[StringLength(36)]
		public string DataConfigID { get; set; }
		/// <summary>
        /// 图层样式ID
		/// </summary>
		[StringLength(36)]
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

    public class StyleInputDto : IInputDto
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        public string MapId { get; set; }
        /// <summary>
        /// 图层ID
        /// </summary>
        public string LayerStr { get; set; }
        /// <summary>
        /// 样式ID
        /// </summary>
        public string StyleStr { get; set; }
    }
}

