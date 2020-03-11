using System;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class GroupDto : EntityDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户组名称
        /// </summary>
        public string Name { get; set; }  
    }
}
