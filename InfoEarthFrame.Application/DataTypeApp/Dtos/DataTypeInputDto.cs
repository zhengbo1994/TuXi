using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.DataTypeApp.Dtos
{
	public class DataTypeInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
		/// 类别名称
		/// </summary>
		[StringLength(50)]
		public string TypeName { get; set; }
		/// <summary>
		/// 类别描述
		/// </summary>
		[StringLength(100)]
		public string TypeDesc { get; set; }
		/// <summary>
        /// 数据类型(地图类／图层类)
		/// </summary>
		[StringLength(36)]
		public string DictCodeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string ParentID { get; set; }

	}
}

