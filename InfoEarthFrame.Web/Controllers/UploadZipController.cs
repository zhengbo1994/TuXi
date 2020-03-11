using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InfoEarthFrame.Common;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System;
using System.Linq;
using System.Configuration;
using InfoEarthFrame.ServerInterfaceApp;

namespace InfoEarthFrame.Web.Controllers
{
    //[Authorize]
    public class UploadZipController : InfoEarthFrameControllerBase
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Upload()
        {
            //接收参数
            string typeCode = Request.Form["Type"]; //type说明,1:mapgis转Shp文件，2:arcgis文件入库，3:mapgis文件入库
            string filePath = Request.Form["FilePath"]; //本地文件路径，多个文件;间隔
            string fileType = Request.Form["FileType"]; //type说明,1:压缩文件，2:其它文件

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

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    fileName = Path.Combine(mapGISUnZipPath, DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName));
                    file.SaveAs(fileName);

                    fileName = Path.GetFileName(fileName);
                }
                else
                {
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
                }

                var serverInterfaceAppService = IocResolver.Resolve<IServerInterfaceAppService>();

                if (!string.IsNullOrEmpty(fileName))
                {
                    switch (typeCode)
                    {
                        case "1":
                            return serverInterfaceAppService.GetMapGISToArcGIS(fileName);
                        case "2":
                            return serverInterfaceAppService.GetArcGISToDB(fileName);
                        case "3":
                            return serverInterfaceAppService.GetMapGISToDB(fileName);
                        default: break;
                    }
                }
            }
            catch (Exception e)
            {
                return "error";
            }
            return "error";
        }

        /// <summary>
        /// 分片上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool BlockUpload()
        {
            int index = Convert.ToInt32(Request.Form["chunk"]);//当前分块序号
            var guid = Request.Form["guid"] + "_" + Request.Files[0].FileName.Replace(".", "_");
            var path = Path.Combine(Server.MapPath("~/file"), guid);
            if (!Directory.Exists(path))//判断是否存在此路径，如果不存在则创建
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                //保存文件到根目录 App_Data + 获取文件名称和格式
                var filePath = Path.Combine(path, index.ToString());
                //创建一个追加（FileMode.Append）方式的文件流
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        //读取文件流
                        BinaryReader br = new BinaryReader(Request.Files[0].InputStream);
                        //将文件留转成字节数组
                        byte[] bytes = br.ReadBytes((int)Request.Files[0].InputStream.Length);
                        //将字节数组追加到文件
                        bw.Write(bytes);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        public string FileMerge()
        {
            try
            {
                var fileName = Request.Form["fileName"];
                var guid = Request.Form["guid"] + "_" + fileName.Replace(".", "_");
                string path = Path.Combine(Server.MapPath("~/file"), guid);
                string physicalPath = Path.Combine(Server.MapPath("~/file"), fileName);//文件的物理路径

                //如果存在则覆盖写入
                using (new FileStream(physicalPath, FileMode.Create, FileAccess.Write)) { }

                //这里排序一定要正确，转成数字后排序（字符串会按1 10 11排序，默认10比2小）
                foreach (var filePath in Directory.GetFiles(path).OrderBy(t => int.Parse(Path.GetFileNameWithoutExtension(t))))
                {
                    using (FileStream fs = new FileStream(physicalPath, FileMode.Append, FileAccess.Write))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(filePath);//读取文件到字节数组
                        fs.Write(bytes, 0, bytes.Length);//写入文件
                    }
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(path);

                //COPY文件生成网络路径
                FileInfo fi = new FileInfo(physicalPath);

                //接收参数
                string typeCode = Request.Form["Type"]; //type说明,1:mapgis转Shp文件，2:arcgis文件入库，3:mapgis文件入库

                string typeName = "ArcGIS";
                switch (typeCode)
                {
                    case "1":
                    case "3":
                        typeName = "MapGIS";
                        break;
                }

                string mapGISUnZipPath = Path.Combine(ConfigurationManager.AppSettings["UploadFilePath"].ToString(), typeName);

                if (!Directory.Exists(mapGISUnZipPath))
                {
                    Directory.CreateDirectory(mapGISUnZipPath);
                }
                string newFile = Path.Combine(mapGISUnZipPath, DateTime.Now.ToString("yyyyMMddHHmmss") + fi.Extension);

                System.IO.File.Copy(physicalPath, newFile, true);

                newFile = Path.GetFileName(newFile);

                var serverInterfaceAppService = IocResolver.Resolve<IServerInterfaceAppService>();

                switch (typeCode)
                {
                    case "1":
                        return serverInterfaceAppService.GetMapGISToArcGIS(newFile);
                    case "2":
                        return serverInterfaceAppService.GetArcGISToDB(newFile);
                    case "3":
                        return serverInterfaceAppService.GetMapGISToDB(newFile);
                    default: break;
                }
            }
            catch (Exception e)
            {
                return "error";
            }
            return "error";
        }
    }
}