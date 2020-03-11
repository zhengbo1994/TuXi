using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类TagReleation
    /// </summary>
    [Table("sdms_layer_readlog")]
    public class ShpFileReadLogEntity : Entity<string>
    {
        [MaxLength(36)]
        [Column("id")]
        public override string Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }
        /// <summary>
        /// 图层ID
        /// </summary>
        [MaxLength(36)]
        [Column("layerid")]
        public string LayerID { get; set; }
        /// <summary>
        /// Shp文件名称
        /// </summary>
        [MaxLength(100)]
        [Column("shpfilename")]
        public string ShpFileName { get; set; }
        /// <summary>
        /// 读取状态[0:默认,1:正常,2:异常]
        /// </summary>
        [Column("readstatus")]
        public int? ReadStatus { get; set; }
        /// <summary>
        /// 消息提示
        /// </summary>
        [MaxLength(4000)]
        [Column("message")]
        public string Message { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column("createdt")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 读取开始时间
        /// </summary>
        [Column("readstartdt")]
        public DateTime? ReadStartDT { get; set; }

        /// <summary>
        /// 读取结束时间
        /// </summary>
        [Column("readenddt")]
        public DateTime? ReadEndDT { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(36)]
        [Column("createby")]
        public string CreateBy { get; set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        [Column("msgstatus")]
        public int? MsgStatus { get; set; }

        /// <summary>
        /// 读取日期
        /// </summary>
        [Column("msgreaddt")]
        public DateTime? MsgReadDT { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        [MaxLength(100)]
        [Column("foldername")]
        public string FolderName { get; set; }
    }
}
