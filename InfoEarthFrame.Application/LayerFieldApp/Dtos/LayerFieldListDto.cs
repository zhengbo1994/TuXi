using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerFieldApp.Dtos
{
    public class LayerFieldListDto : IOutputDto
    {
        /// <summary>
        /// 验证消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 图层字段集合
        /// </summary>
        public List<LayerFieldDto> LayerField { get; set; }
    }
}
