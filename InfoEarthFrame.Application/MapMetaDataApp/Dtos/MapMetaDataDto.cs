using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.MapMetaDataApp.Dtos
{
	public class MapMetaDataDto : EntityDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// µØÍ¼ID
		/// </summary>
		public string MapID { get; set; }
		/// <summary>
		/// °æ±¾
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Summary { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Target { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MaintenanceFre { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AdministrativeDivisions { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string NomalLimit { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string OtherLimit { get; set; }
		/// <summary>
		/// 
		/// </summary>
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
		public string MetaDataQualityDesc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ThumbnalAddress { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MetaDataType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string MetaDataTag { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CreateBy { get; set; }
		/// <summary>
		/// 
		/// </summary>
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

