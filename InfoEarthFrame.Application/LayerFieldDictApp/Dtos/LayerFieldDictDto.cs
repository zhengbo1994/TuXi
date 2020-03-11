using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerFieldDictApp.Dtos
{
    [AutoMapFrom(typeof(LayerContentEntity))]
	public class LayerFieldDictDto : EntityDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AttributeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FieldDictName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FieldDictDesc { get; set; }

	}
}

