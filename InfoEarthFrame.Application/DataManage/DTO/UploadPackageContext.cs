using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.DataManage.DTO
{
    public class UploadFileResult
    {
        public string Category { get; set; }

        public int ErrorCode { get; set; }

        public List<string> ErrorInfo { get; set; }
    }
    public class UploadPackageContext
    {



        /// <summary>
        /// 主数据Id
        /// </summary>
        public string MainId { get; set; }

        /// <summary>
        /// 上传用户ID
        /// </summary>
        public string UploadUserId { get; set; }
        /// <summary>
        /// 压缩包文件名
        /// </summary>
        public string ZipFileName { get; set; }

        public string Name
        {
            get
            {
                return ZipFileName.Split('_')[0];
            }
        }
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalFilePath
        {
            get
            {
                return Path.Combine(SaveDirectory, ZipFileName);
            }
        }

        /// <summary>
        /// Ftp文件路径
        /// </summary>
        protected string FtpFilePath { get; private set; }

        /// <summary>
        /// 内部文件分类
        /// </summary>
        public Dictionary<string, string> PackageCategory
        {
            get
            {
                return ConfigContext.Current.PackageCategory;
            }
        }

        /// <summary>
        /// 上传文件保存的文件夹
        /// </summary>
        public string SaveDirectory
        {
            get
            {
                var dir = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], MainId);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
        }

        private List<UploadFileResult> _uploadFileResults;
        /// <summary>
        /// 上传文件结果反馈
        /// </summary>
        public List<UploadFileResult> UploadFileResults
        {
            get
            {
                if (_uploadFileResults == null)
                {
                    _uploadFileResults = new List<UploadFileResult>();
                }
                return _uploadFileResults;
            }
        }


        /// <summary>
        /// 解压文件
        /// </summary>
        public void UnzipFile()
        {
            var ext = Path.GetExtension(LocalFilePath);
            if (ext.ToLower() == ".zip" || ext.ToLower() == ".rar")
            {
                RarOrZipUtil.DeCompress(LocalFilePath, SaveDirectory);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public void Upload()
        {
           
        }
    }
}
