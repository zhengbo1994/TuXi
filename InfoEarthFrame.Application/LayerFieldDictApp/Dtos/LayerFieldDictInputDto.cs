using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.LayerFieldDictApp.Dtos
{
	public class LayerFieldDictInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(128)]
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string AttributeID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string FieldDictName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string FieldDictDesc { get; set; }

	}
}

