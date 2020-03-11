using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class MultimediaTypeDto : EntityDto
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 业务模块的类别
        /// </summary>
        public string ModuleType { get; set; }
    }
}
