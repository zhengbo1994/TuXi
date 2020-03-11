using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace InfoEarthFrame.Application
{
    public class GroupUserComparaInput : IInputDto
    {
        /// <summary>
        /// 已存在用户
        /// </summary>
        public List<GroupUserInput> ExistUser { get; set; }

        /// <summary>
        /// 需添加用户
        /// </summary>
        public List<GroupUserInput> InputArr { get; set; }  
    }

    public class GroupUserDto
    {


        public string GroupId { get; set; }

        public List<string> UserIds { get; set; }
    }
}