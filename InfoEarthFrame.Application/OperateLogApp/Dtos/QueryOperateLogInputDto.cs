using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using System.Xml.Serialization;

namespace InfoEarthFrame.Application.OperateLogApp.Dtos
{
    public class QueryOperateLogInputParamDto
    {
        public string UserName { get; set; }
        public string SystemFunc { get; set; }
        public string OperateType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class QueryOperateLogInputDto : IInputDto
    {
        /// <summary>
        /// 操作用户（UserCode）
        /// </summary>
        public string OperateId { get; set; }

        /// <summary>
        /// 要查询的用户（UserCode）
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 系统功能（1-图层管理；2-地图管理；3-样式管理；null-无限制）
        /// </summary>
        public string SystemFunc { get; set; }

        /// <summary>
        /// 操作类型（1-新增；2-编辑；3-导入；4-清空；5-删除；6-刷新；null-无限制）
        /// </summary>
        public string OperateType { get; set; }
    }


    public class QueryUserInputDto : IInputDto
    {
        /// <summary>
        /// 区域编号
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }


    public class TranslateOutput
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Chinese")]
        public string Chinese { get; set; }

        [XmlAttribute("English")]
        public string English { get; set; }
    }
}
