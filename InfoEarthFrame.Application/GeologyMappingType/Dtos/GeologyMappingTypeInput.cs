using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace InfoEarthFrame.Application
{
    public class GeologyMappingTypeInput:IInputDto
    {
        /// <summary>
        /// 序号ID
        /// </summary>
        ///  
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
         [Required]
        public string ParentID { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
         [Required]
        public string Paths { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string ClassName { get; set; }
    }
}
