using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Core
{
    [Table("Attachment")]
    public class AttachmentEntity : Entity<string>
    {
        /// <summary>
        /// 外键
        /// </summary>
        [MaxLength(50)]
        public string FKey { get; set; }

        /// <summary>
        /// 物理名
        /// </summary>
        [MaxLength(100)]
        public string PhysicalName { get; set; }

        /// <summary>
        /// 逻辑名
        /// </summary>
        [MaxLength(100)]
        public string LogicName { get; set; }

        /// <summary>
        /// 物理路径
        /// </summary>
        [MaxLength(500)]
        public string PhysicalPath { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        [MaxLength(500)]
        public string HttpPath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        [MaxLength(10)]
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
        [MaxLength(50)]
        public string CreateName { get; set; }

        /// <summary>
        /// 业务模块的多媒体附件类别ID,对应MultimediaType表主键
        /// </summary>
        [MaxLength(50)]
        public string TypeCode { get; set; }

    }
}
