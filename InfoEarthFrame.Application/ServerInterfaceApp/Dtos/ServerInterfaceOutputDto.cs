using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    public class ServerInterfaceOutputDto
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string ServerDesc { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType { get; set; }
        /// <summary>
        /// 服务路径
        /// </summary>
        public string ServerPath { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? ServerSort { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string ServerKeyWord { get; set; }
        /// <summary>
        /// 服务请求参数列表
        /// </summary>
        public ListResultOutput<ParameterOutputDto> ServerRequestParameters { get; set; }
        /// <summary>
        /// 服务返回参数列表
        /// </summary>
        public ListResultOutput<ParameterOutputDto> ServerResponseParameters { get; set; }
    }
}
