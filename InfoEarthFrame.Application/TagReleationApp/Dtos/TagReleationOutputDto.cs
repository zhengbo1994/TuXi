using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.TagReleationApp.Dtos
{
	[AutoMapFrom(typeof(TagReleationEntity))]
	public class TagReleationOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
        /// ±Í«©ID
		/// </summary>
		public string DataTagID { get; set; }
		/// <summary>
        /// µÿÕºªÚÕº≤„ID
		/// </summary>
		public string MapID { get; set; }

	}
}

