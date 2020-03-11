using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DataManage.DTO
{
    public class DataMainDto
    {
        public string Id { get; set; }
        public string MappingTypeID { get; set; }

        /// <summary>
        /// 比例尺分母
        /// </summary>
        public int? Scale { get; set; }


        /// <summary>
        /// ZIP文件名
        /// </summary>
        public string ZipFileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 主要SHP文件名
        /// </summary>
        public string MainShpFileName { get; set; }

        /// <summary>
        /// 版本 1 验收版 和 2 最终版
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int VersionNo { get; set; }


        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? StorageTime { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? ReleaseTime { get; set; }

        /// <summary>
        /// 入库人编号
        /// </summary>
        public string CreaterID { get; set; }

        /// <summary>
        /// 入库人
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }

        public string Name { get; set; }
    }
}