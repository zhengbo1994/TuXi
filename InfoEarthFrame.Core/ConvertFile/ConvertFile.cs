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
    /// 转换文件
    /// </summary>
    [Table("sdms_convertfile")]
    public class ConvertFile : Entity<string>
    {

        /// <summary>
        /// 用户标识
        /// </summary>
        [MaxLength(50)]
        public string UserID { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>

        public int FileType { get; set; }  // 1:格式 2:坐标 3:投影
        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(100)]
        public string FileName { get; set; }
        /// <summary>
        /// 物理路径
        /// </summary>
        [MaxLength(200)]
        public string PhysicsFilePath { get; set; }

        /// <summary>
        /// 生成文件的详细列表
        /// </summary>
        [MaxLength(500)]
        public string ConvertFileNames { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int STATE { get; set; }
    }

    public enum DataFileType
    {
        FormatConvert = 1, // 格式
        CoordinateConvert = 2, // 坐标
        Projection = 3 // 投影

    }

    public class ConvertResult
    {
        public string ZipFileName { get; set; }
        public List<ConvertFileList> fileList { get; set; }
    }

    public class ConvertFileList
    {
        public string ID { get; set; } // GUID
        public int FileType { get; set; } // 文件类型
        public string LogicFileName { get; set; } // 逻辑文件名
        public string PhysicsFilePath { get; set; } // 物理文件路径
        public string ConvertFilePath { get; set; } // 转换后文件路径

        public string ConvertFolder { get; set; }
        public int ConvertResult { get; set; } // 转换结果 0：失败 1：成功
        public string ConvertMsg { get; set; } // 转换信息
        public string SrcCoordName { get; set; } // 原空间参考（传入）
        public string CoordName { get; set; } // 空间参考（转换后）
        public string WKT { get; set; } // 空间参考（转换后）
        public string ConvertKey { get; set; }//转换关键字
        public string[] CoordPoint { get; set; }//坐标点字符串
    }
}
