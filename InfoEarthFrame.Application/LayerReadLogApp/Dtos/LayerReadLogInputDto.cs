using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.LayerReadLogApp.Dtos
{
	public class LayerReadLogInputDto : IInputDto
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
		[StringLength(36)]
		public string LayerID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(100)]
		public string ShpFileName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? ReadStatus { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Required]
		[StringLength(4000)]
		public string Message { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Required]
        [StringLength(36)]
        public string CreateBy { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ReadStartDT { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ReadEndDT { get; set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        public int? MsgStatus { get; set; }
        /// <summary>
        /// 读取日期
        /// </summary>
        public DateTime? MsgReadDT { get; set; }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FolderName { get; set; }
	}
}

