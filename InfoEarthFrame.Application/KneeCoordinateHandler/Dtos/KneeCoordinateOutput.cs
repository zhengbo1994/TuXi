using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    public class KneeCoordinateOutput
    {
        /// <summary>
        /// 状态（true:成功，false:失败）
        /// </summary>
        public bool status { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<KneeCoordinateModal> data { get; set; }
    }
}
