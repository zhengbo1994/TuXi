using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.OperateLogApp.Dtos
{
    public class OperateLogDto : EntityDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperateTime { get; set; }

        /// <summary>
        /// 操作用户（UserName）
        /// </summary>
        [MaxLength(36)]
        public string UserName { get; set; }

        /// <summary>
        /// 操作用户ID（UserCode）
        /// </summary>
        [MaxLength(36)]
        public string UserCode { get; set; }

        /// <summary>
        /// 系统功能
        /// </summary>
        [MaxLength(15)]
        public string SystemFunc { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [MaxLength(36)]
        public string OperateType { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [MaxLength(36)]
        public string Result { get; set; }

        /// <summary>
        /// 执行结果描述
        /// </summary>
        [MaxLength(100)]
        public string ResultDescribe { get; set; }

        /// <summary>
        /// 图层ID
        /// </summary>
        [MaxLength(36)]
        public string LayerID { get; set; }
    }
}
