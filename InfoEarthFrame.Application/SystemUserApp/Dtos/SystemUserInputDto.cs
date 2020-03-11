using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.SystemUserApp.Dtos
{
	public class SystemUserInputDto : IInputDto
	{
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(128)]
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(50)]
		public string UserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(50)]
		public string UserCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(5)]
		public string UserSex { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(50)]
		public string Password { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(20)]
		public string TelPhone { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(20)]
		public string Phone { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(100)]
		public string Department { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(100)]
        public string Position { get; set; }
        /// <summary>
        /// หตร๗
        /// </summary>
        [StringLength(4000)]
        public string Remark { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }

	}
}

