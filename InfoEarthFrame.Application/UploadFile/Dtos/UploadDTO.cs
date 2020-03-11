using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class UploadDTO : EntityDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
}
