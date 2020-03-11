using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.SystemUserApp.Dtos
{
	[AutoMapFrom(typeof(SystemUserEntity))]
	public class SystemUserOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UserCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UserSex { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string TelPhone { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Phone { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Department { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Position { get; set; }
        /// <summary>
        /// หตร๗
        /// </summary>
        public string Remark { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }

        public string AreaFullName
        {
            get;
            set;
        }

        public string[] GroupIds { get; set; }

	}
}

