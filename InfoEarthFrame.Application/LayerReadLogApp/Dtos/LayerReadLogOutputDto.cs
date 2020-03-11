using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerReadLogApp.Dtos
{
    [AutoMapFrom(typeof(ShpFileReadLogEntity))]
	public class LayerReadLogOutputDto : IOutputDto
	{
		/// <summary>
		/// 
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string LayerID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LayerName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ShpFileName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int? ReadStatus { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
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

