using System.Security.Claims;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Configuration;
using System.Web.Http;
using System.Net.WebSockets;


namespace InfoEarthFrame.Web.Controllers
{
    //[Authorize]

    public class FileUploadDTO
    {
        /// <summary>
        /// 附件唯一标识
        /// </summary>
        public string FileGuid { get; set; }
        /// <summary>
        /// 物理文件名
        /// </summary>
        public string PhysicalName { get; set; }
        /// <summary>
        /// 逻辑文件名
        /// </summary>
        public string LogicName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }
        /// <summary>
        /// 扩展名 例：".txt"
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 物理路径
        /// </summary>
        public string PhysicalPath { get; set; }
        /// <summary>
        /// 网络路径
        /// </summary>
        public string HttpPath { get; set; }
    }
    public class UploadForBreakpointController : InfoEarthFrameControllerBase
    {
        /// <summary>
        /// 分片上传文件
        /// </summary>
        /// <returns></returns>
         [HttpPost]
        public bool AddFiles()
        {
            int index = Convert.ToInt32(Request.Form["chunk"]);//当前分块序号
            var guid = Request.Form["guid"] +"_"+ Request.Files[0].FileName.Replace(".","_");
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/file"), guid);
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
        public System.Web.Mvc.JsonResult FileMerge()
        {
            var fileName = Request.Form["fileName"];
            var guid = Request.Form["guid"] + "_" + fileName.Replace(".", "_");
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/file"), guid);
            string physicalPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/file"), fileName);//文件的物理路径

             //如果存在则覆盖写入
            using (new FileStream(physicalPath, FileMode.Create, FileAccess.Write)){}

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
            string fileExtension = fi.Extension;
            guid = Guid.NewGuid().ToString();
           // string newName = guid + fileExtension;

            string newName = Path.GetFileName(fileName);
            string newFile = ConfigurationManager.AppSettings["UploadFilePath"] + newName;
            var dir = Path.GetDirectoryName(physicalPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            System.IO.File.Copy(physicalPath, newFile,true);

            //返回文件信息
            FileUploadDTO file = new FileUploadDTO {
                FileGuid = guid,
                Extension = fileExtension,
                FileSize = fi.Length.ToString(),
                PhysicalName=fileName,
                PhysicalPath=physicalPath,
                LogicName = newName,
                HttpPath = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + newFile.Replace(Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath), "").Replace(@"\", @"/")  
            };
            return Json(file);
        }

    }
}