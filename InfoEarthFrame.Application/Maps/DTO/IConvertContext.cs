using InfoEarthFrame.Application;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Maps.DTO
{
    public interface IConvertContext
    {
        IDataConvertAppService ConvertService { get; }
        /// <summary>
        /// 唯一标识
        /// </summary>
         string MainId { get; }

        /// <summary>
        /// 保存的文件夹
        /// </summary>
        string SaveDirectory { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        IList<string> ErrorInfo { get; }

        /// <summary>
        /// 转换后的结果
        /// </summary>
        ConvertResult ConvertResult { get; set; }

        /// <summary>
        /// 要转换的文件
        /// </summary>
        IList<ConvertFileList> ConvertFileList { get; }

        /// <summary>
        /// 转换之前
        /// </summary>
        /// <returns></returns>
        void BeforeConvert();

        /// <summary>
        /// 执行转换
        /// </summary>
        /// <returns></returns>
        void Convert();

        bool IsSuccess { get; }
        /// <summary>
        /// 转换之后
        /// </summary>
        /// <returns></returns>
        void AfterConvert();

        /// <summary>
        /// 压缩文件相对路径
        /// </summary>
        string RarFileRelativePath { get; set; }

        /// <summary>
        /// 压缩文件名称
        /// </summary>
        string RarFileName { get; }

        /// <summary>
        /// 压缩文件所在文件夹
        /// </summary>
        string RarFileDirectory { get; }
    }
}
