using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class DisasterInput : IInputDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 灾害点名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DISASTERNAME { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime OCCURTIME { get; set; }

        /// <summary>
        /// 具体位置
        /// </summary>
         [StringLength(200)]
        public string POSITION { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
         [StringLength(500)]
        public string REMARK { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
         [StringLength(100)]
        public string AREARIGHTCODE
        {
            get;
            set;
        }

         [StringLength(100)]
        public bool IsDeleted
        {
            get;
            set;
        }
    }
}
