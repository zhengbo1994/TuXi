using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.DataTagApp.Dtos
{
	public class DataTagDto : EntityDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 标签名称
		/// </summary>
		public string TagName { get; set; }
		/// <summary>
		/// 标签描述
		/// </summary>
		public string TagDesc { get; set; }
		/// <summary>
		/// 数据类型(地图类／图层类)
		/// </summary>
		public string DictCodeID { get; set; }

	}
}

