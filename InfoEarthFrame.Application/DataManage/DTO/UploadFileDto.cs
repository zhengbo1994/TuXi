using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.DataManage.DTO
{
    public class UploadLayerDto
    {
        //TODO:点线面以及类型不知道,后期可能要同步数据
        public string Id { get; set; }

        public string LayerName { get; set; }

        public int UploadStatus { get; set; }

        public string CreateBy { get; set; }

        public int UploadFileType { get; set; }

        /// <summary>
        /// 主数据ID
        /// </summary>
        public string MainID { get; set; }

        public string LayerContentId { get; set; }
    }
    public class UploadFileDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string Ext { get; set; }


        /// <summary>
        /// 文件长度
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件路径（FTP）
        /// </summary>
        public string FileData { get; set; }

        /// <summary>
        /// 主数据ID
        /// </summary>
        public string MainID { get; set; }
    }
}
