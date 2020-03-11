using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DicDataCodeApp.Dtos
{
	[AutoMapFrom(typeof(DicDataCodeEntity))]
	public class DicDataCodeOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
        /// 基本代码名称
		/// </summary>
		public string CodeName { get; set; }
		/// <summary>
        /// 基本代码值
		/// </summary>
		public string CodeValue { get; set; }
		/// <summary>
        /// 代码描述
		/// </summary>
		public string CodeDesc { get; set; }
		/// <summary>
        /// 代码类型
		/// </summary>
		public string DataTypeID { get; set; }
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
        /// 代码排序
        /// </summary>
        public int? CodeSort { get; set; }
		/// <summary>
        /// 备注
		/// </summary>
		public string Remark { get; set; }
		/// <summary>
        /// 关键字
		/// </summary>
		public string Keywords { get; set; }

	}
}

