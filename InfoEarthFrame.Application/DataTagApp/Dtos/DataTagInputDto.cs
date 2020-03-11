using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.DataTagApp.Dtos
{
	public class DataTagInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 标签名称
		/// </summary>
		[StringLength(50)]
		public string TagName { get; set; }
		/// <summary>
        /// 标签描述
		/// </summary>
		[StringLength(100)]
		public string TagDesc { get; set; }
		/// <summary>
        /// 数据类型(地图类／图层类)
		/// </summary>
		[StringLength(36)]
		public string DictCodeID { get; set; }

	}
}

