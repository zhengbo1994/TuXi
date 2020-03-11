using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.SystemUserApp.Dtos
{
	public class SystemUserDto 
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

        public string GroupId { get; set; }

        public int IsRelated { get; set; }

        public bool LAY_CHECKED
        {
            get
            {
                return IsRelated == 1;
            }
        }
	}


}

