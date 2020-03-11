using System;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class GroupUserInput : IInputDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }  
    }
}