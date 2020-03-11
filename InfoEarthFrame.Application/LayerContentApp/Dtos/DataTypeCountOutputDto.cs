using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.LayerContentApp.Dtos
{
    public class DataTypeCountOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public List<string> Names { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public List<string> Colors { get; set; }

        /// <summary>
        /// 统计
        /// </summary>
        public List<DataTypeCountDto> Data { get; set; }
    }
    public class DataTypeCountDto
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; } 

        /// <summary>
        /// 统计
        /// </summary>
        public int Value { get; set; }
    }
}
