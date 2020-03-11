using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.DicDataTypeApp.Dtos
{
	public class DicDataTypeInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
		/// 类型名称
		/// </summary>
		[StringLength(50)]
		public string TypeName { get; set; }
		/// <summary>
		/// 类型描述
		/// </summary>
		[StringLength(100)]
		public string TypeDesc { get; set; }
		/// <summary>
		/// 类型代码
		/// </summary>
		[StringLength(36)]
		public string TypeCode { get; set; }
		/// <summary>
		/// 类型排序
		/// </summary>
		public int? TypeSort { get; set; }
		/// <summary>
		/// 父类型
		/// </summary>
		[StringLength(36)]
		public string ParentID { get; set; }
		/// <summary>
        /// 关键字
		/// </summary>
		[StringLength(100)]
		public string Keywords { get; set; }

	}
}

