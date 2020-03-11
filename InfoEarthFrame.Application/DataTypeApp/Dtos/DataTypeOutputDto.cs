using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DataTypeApp.Dtos
{
	[AutoMapFrom(typeof(DataTypeEntity))]
	public class DataTypeOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 类别名称
		/// </summary>
		public string TypeName { get; set; }
		/// <summary>
		/// 类别描述
		/// </summary>
		public string TypeDesc { get; set; }
		/// <summary>
        /// 数据类型(地图类／图层类)
		/// </summary>
		public string DictCodeID { get; set; }
		/// <summary>
		/// 父类别
		/// </summary>
		public string ParentID { get; set; }

	}
}

