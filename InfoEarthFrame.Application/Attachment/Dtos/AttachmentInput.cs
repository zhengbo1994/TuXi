using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class AttachmentInput : IInputDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string FKey { get; set; }
        /// <summary>
        /// 物理名
        /// </summary>
        public string PhysicalName { get; set; }

        /// <summary>
        /// 逻辑名
        /// </summary>
        public string LogicName { get; set; }
        /// <summary>
        /// 物理路径
        /// </summary>
        public string PhysicalPath { get; set; }
        /// <summary>
        /// 网络路径
        /// </summary>
        public string HttpPath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int? Sn { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public byte? iState { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }
    }
}
