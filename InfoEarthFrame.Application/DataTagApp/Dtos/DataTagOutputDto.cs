using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DataTagApp.Dtos
{
	[AutoMapFrom(typeof(DataTagEntity))]
	public class DataTagOutputDto : IOutputDto
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

