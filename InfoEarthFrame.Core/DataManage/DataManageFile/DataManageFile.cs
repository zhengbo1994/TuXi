using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InfoEarthFrame.Core
{
    /// <summary>
    /// 数据管理（文件）
    /// </summary>
    [Table("TBL_DATAMANAGEFILE")]
    public class DataManageFile : Entity<string>
    {
        /// <summary>
        /// 所属数据管理
        /// </summary>
        [MaxLength(50)]
        public string MainID { get; set; }

        /// <summary>
        /// 目录名
        /// </summary>
        [MaxLength(150)]
        public string FolderName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(100)]
        public string FileName { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        [MaxLength(10)]
        public string Ext { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int? FileSize { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public string FileData { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? StorageTime { get; set; }

        /// <summary>
        /// 入库人编号
        /// </summary>
        [MaxLength(50)]
        public string CreaterID { get; set; }

        /// <summary>
        /// 入库人
        /// </summary>
        [MaxLength(30)]
        public string Creater { get; set; }


        public string ParentId { get; set; }
    }
}
