using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class ModuleInfoInput : IInputDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string FKey { get; set; }

        /// <summary>
        /// 业务模块的类别
        /// </summary>
        public string ModuleType { get; set; }
    }
}
