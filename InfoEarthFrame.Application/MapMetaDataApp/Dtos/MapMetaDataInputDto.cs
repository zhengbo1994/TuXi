using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.MapMetaDataApp.Dtos
{
	public class MapMetaDataInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string MapID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Version { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string Summary { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string Target { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string MaintenanceFre { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(200)]
		public string AdministrativeDivisions { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(50)]
		public string NomalLimit { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string OtherLimit { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(200)]
		public string SpatialGeographical { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? StartDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? EndDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(200)]
		public string AdditionalInfo { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? PublishDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ModifyDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(200)]
		public string MetaDataQualityDesc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string ThumbnalAddress { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(100)]
		public string MetaDataType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(200)]
		public string MetaDataTag { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string CreateBy { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[StringLength(36)]
		public string Owner { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? IsPublish { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }

	}
}

