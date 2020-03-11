using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class DownloadController : BaseApiController
    {
        private readonly IDataManageAppService _dataManageAppService;
        public DownloadController(IDataManageAppService dataManageAppService)
        {
            this._dataManageAppService = dataManageAppService;
        }


        /// <summary>
        /// 获取图层文件
        /// </summary>
        /// <param name="mainId">主数据ID</param>
        /// <returns></returns>
        public HttpResponseMessage Getlayers(string mainId)
        {
            var ftp = ConfigContext.Current.FtpConfig["package"];
            var files=new List<string>();

            var layers=_dataManageAppService.GetFileList(new GetDataFileListParamDto
            {
                FolderName = @"6要素类文件\专业图层",
                MainID = mainId
            }).Select(p =>ftp.DirectoryPath+p.FilePath.Replace(ftp.Site,"").Replace("/", "\\"));
            files.AddRange(layers);

             layers = _dataManageAppService.GetFileList(new GetDataFileListParamDto
            {
                FolderName = @"6要素类文件\地理图层",
                MainID = mainId
            }).Select(p => ftp.DirectoryPath + p.FilePath.Replace(ftp.Site, "").Replace("/", "\\"));
             files.AddRange(layers);


             return GetItems(mainId, files, ftp);
        }

        /// <summary>
        /// 获取成果图
        /// </summary>
        /// <param name="mainId">主数据id</param>
        /// <returns></returns>
        public HttpResponseMessage GetResultMap(string mainId)
        {
            var ftp = ConfigContext.Current.FtpConfig["package"];
             var files = new List<string>();
            var images = _dataManageAppService.GetFileList(new GetDataFileListParamDto
            {
                FolderName = "3栅格图",
                MainID = mainId
            }).Select(p => ftp.DirectoryPath + p.FilePath.Replace(ftp.Site, "").Replace("/", "\\"));
            files.AddRange(images);
            return GetItems(mainId, files, ftp);
        }

        /// <summary>
        /// 获取制图文件(tif格式)
        /// </summary>
        /// <param name="mainId">主数据id</param>
        /// <returns></returns>
        public HttpResponseMessage GetDrawingOutput(string id)
        {
            var ftp = ConfigContext.Current.FtpConfig["DrawingOutput"];
            var filePath = string.Format(@"{0}\DrawingOutput\{1}.tif", ftp.DirectoryPath, id);
            return GetFile(filePath, Path.GetFileName(filePath));
        }


        /// <summary>
        /// 将FTP文件夹中的文件压缩并提供下载
        /// </summary>
        /// <param name="mainId">主数据ID</param>
        /// <param name="files">文件列表</param>
        /// <param name="ftp">FTP配置</param>
        /// <returns></returns>
        private HttpResponseMessage GetItems(string mainId,IEnumerable<string> files,Ftp ftp)
        {
            files = files.Distinct().ToList();
            var rarName = mainId + ".zip";
            var rarFilePath = string.Format(@"{0}\Package\{1}\{2}.zip", ftp.DirectoryPath, mainId, mainId);
            RarOrZipUtil.Compress(files, rarFilePath);
            return GetFile(rarFilePath, rarName);
        }
    }
}
