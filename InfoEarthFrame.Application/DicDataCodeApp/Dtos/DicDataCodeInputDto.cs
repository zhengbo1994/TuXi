using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.DicDataCodeApp.Dtos
{
	public class DicDataCodeInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
        /// 基本代码名称
		/// </summary>
		[StringLength(50)]
		public string CodeName { get; set; }
		/// <summary>
        /// 基本代码值
		/// </summary>
		[StringLength(4000)]
		public string CodeValue { get; set; }
		/// <summary>
        /// 代码描述
		/// </summary>
		[StringLength(100)]
		public string CodeDesc { get; set; }
		/// <summary>
        /// 代码类型
		/// </summary>
		[StringLength(36)]
		public string DataTypeID { get; set; }
		/// <summary>
        /// 代码排序
		/// </summary>
		public int? CodeSort { get; set; }
		/// <summary>
        /// 备注
		/// </summary>
		[StringLength(200)]
		public string Remark { get; set; }
		/// <summary>
        /// 关键字
		/// </summary>
		[StringLength(100)]
		public string Keywords { get; set; }

	}
}

