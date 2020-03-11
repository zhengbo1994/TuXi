using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.ShpFileReadLogApp.Dtos
{
    [AutoMapFrom(typeof(ShpFileReadLogEntity))]
    public class ShpFileReadLogOutputDto : IOutputDto
    {
        [MaxLength(36)]
        public string Id { get; set; }
        /// <summary>
        /// 图层ID
        /// </summary>
        [MaxLength(36)]
        public string LayerID { get; set; }
        /// <summary>
        /// Shp文件名称
        /// </summary>
        [MaxLength(100)]
        public string ShpFileName { get; set; }
        /// <summary>
        /// 读取状态[0:默认,1:正常,2:异常]
        /// </summary>
        public int? ReadStatus { get; set; }
        /// <summary>
        /// 消息提示
        /// </summary>
        [MaxLength(4000)]
        public string Message { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 读取开始时间
        /// </summary>
        public DateTime? ReadStartDT { get; set; }

        /// <summary>
        /// 读取结束时间
        /// </summary>
        public DateTime? ReadEndDT { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(36)]
        public string CreateBy { get; set; }
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
        [MaxLength(100)]
        public string FolderName { get; set; }
    }
}
