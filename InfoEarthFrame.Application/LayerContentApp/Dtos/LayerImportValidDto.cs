using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerContentApp.Dtos
{
    public class LayerImportValidDto : IOutputDto
    {
        /// <summary>
        /// 验证状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 验证消息
        /// </summary>
        public string Message { get; set; }
    }
}
