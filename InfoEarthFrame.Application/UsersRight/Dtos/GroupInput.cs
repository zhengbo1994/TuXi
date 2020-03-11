using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class GroupInput : IInputDto
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
