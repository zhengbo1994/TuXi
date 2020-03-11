using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DicDataTypeApp.Dtos
{
	[AutoMapFrom(typeof(DicDataTypeEntity))]
	public class DicDataTypeOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName { get; set; }
		/// <summary>
		/// 类型描述
		/// </summary>
		public string TypeDesc { get; set; }
		/// <summary>
		/// 类型代码
		/// </summary>
		public string TypeCode { get; set; }
		/// <summary>
		/// 类型排序
		/// </summary>
		public int? TypeSort { get; set; }
		/// <summary>
		/// 父类型
		/// </summary>
		public string ParentID { get; set; }
		/// <summary>
		/// 关键字
		/// </summary>
		public string Keywords { get; set; }

	}
}

