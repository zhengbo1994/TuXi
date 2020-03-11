using InfoEarthFrame.Common;
using InfoEarthFrame.DataManage.DTO;
using InfoEarthFrame.ServerInterfaceApp;
using InfoEarthFrame.WebApi.Next.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
 
    public class UploadZipController : BaseApiController
    {
        private readonly IServerInterfaceAppService _serverInterfaceAppService;
        public UploadZipController(IServerInterfaceAppService serverInterfaceAppService)
        {
            this._serverInterfaceAppService = serverInterfaceAppService;
        }

        /// <summary>
        /// 上传arcgis图层文件到服务器
        /// </summary>
        /// <param name="dto">图层数据</param>
        /// <returns></returns>
        public HttpResponseMessage  Upload([FromBody]UploadZipDto dto)
        {
            var resp = new HttpResponseMessage();
            //接收参数
            string typeCode = dto.Type; //type说明,1:mapgis转Shp文件，2:arcgis文件入库，3:mapgis文件入库
            string filePath = dto.FilePath; //本地文件路径，多个文件;间隔
            string fileType = dto.FileType; //type说明,1:压缩文件，2:其它文件

            try
            {
                string typeName = "ArcGIS";
                switch (typeCode)
                {
                    case "1":
                    case "3":
                        typeName = "MapGIS";
                        break;
                }
                string fileName = string.Empty;
                string mapGISUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), typeName);

                //if ( HttpContext.Current.Request.Files.Count > 0)
                //{
                //    var file = HttpContext.Current.Request.Files[0];
                //    fileName = Path.Combine(mapGISUnZipPath, DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName));
                //    file.SaveAs(fileName);

                //    fileName = Path.GetFileName(fileName);
                //}
                //else
                //{
                    if (!Directory.Exists(mapGISUnZipPath))
                    {
                        Directory.CreateDirectory(mapGISUnZipPath);
                    }
                    if (fileType == "1" && System.IO.File.Exists(filePath))
                    {
                        fileName = Path.Combine(mapGISUnZipPath, DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(filePath));
                        System.IO.File.Copy(filePath, fileName, true);
                        fileName = Path.GetFileName(fileName);
                    }
                    else
                    {
                        string foldName = Path.Combine(mapGISUnZipPath, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        if (!Directory.Exists(foldName))
                        {
                            Directory.CreateDirectory(foldName);
                        }
                        ZipHelper zip = new ZipHelper();
                        string[] files = filePath.Split(';');
                        foreach (string file in files)
                        {
                            if (System.IO.File.Exists(file))
                            {
                                System.IO.File.Copy(file, Path.Combine(foldName, Path.GetFileName(file)), true);
                            }
                        }
                        bool success = zip.ZipDir(foldName, foldName + ".zip");
                        if (success)
                        {
                            fileName = Path.GetFileName(foldName + ".zip");
                        }
                    }
               // }


                    if (!string.IsNullOrEmpty(fileName))
                    {
                        switch (typeCode)
                        {
                            case "1":
                                var s = _serverInterfaceAppService.GetMapGISToArcGIS(fileName);
                                resp.Content = new StringContent(s);
                                resp.StatusCode = HttpStatusCode.OK;
                                return resp;
                            case "2":
                                var s1 = _serverInterfaceAppService.GetArcGISToDB(fileName);
                             resp.Content = new StringContent(s1);
                                resp.StatusCode = HttpStatusCode.OK;
                                return resp;
                            case "3":
                                var s2 = _serverInterfaceAppService.GetMapGISToDB(fileName);
                                 resp.Content = new StringContent(s2);
                                resp.StatusCode = HttpStatusCode.OK;
                                return resp;
                            default:
                                throw new Exception(" invalid typeCode");
                        }
                    }
                    else
                    {
                        throw new Exception(" invalid fileName");
                    }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
