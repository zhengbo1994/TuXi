using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InfoEarthFrame.Common;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System;
using System.Configuration;

namespace InfoEarthFrame.Web.Controllers
{
    //[Authorize]
    public class UploadController : InfoEarthFrameControllerBase
    {
        [HttpPost]
        public ActionResult index()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            JsonResult ret = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            try
            {
                Response.ContentType = "text/plain";
                Response.Charset = "utf-8";
                HttpPostedFileBase file = Request.Files["file"];
                string filePath = Server.MapPath("\\file") + "\\";
                string newfilePath = Server.MapPath("\\UploadFilePath") + "\\";

                if (file != null)
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    if (!Directory.Exists(newfilePath))
                    {
                        Directory.CreateDirectory(newfilePath);
                    }
                }
                else
                {
                    dic.Add("success", "false");
                    dic.Add("message", "请选择需要上传的文件");
                    ret.Data = dic;
                    return ret;
                }

                string fileExtension = Path.GetExtension(file.FileName);
                string fileName = file.FileName;

                var path = filePath + fileName;
                file.SaveAs(path);

                FileInfo fi = new FileInfo(path);
                string newName = file.FileName;
                string newFile = newfilePath + newName; 
                System.IO.File.Copy(path, newFile, true);

                dic.Add("success", "true");
                dic.Add("message", "文件保存成功");
                dic.Add("physicalName", fileName);
                dic.Add("logicName", newName);
                dic.Add("fileSize", file.ContentLength.ToString());
                dic.Add("extension", fileExtension);
                dic.Add("physicalPath", newFile);
                dic.Add("httpPath", Request.Url.Scheme + "://" + Request.Url.Authority + "/" + newFile.Replace(Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath), "").Replace(@"\", @"/"));
            }
            catch (Exception exception)
            {
                dic = new Dictionary<string, string>();
                dic.Add("success", "false");
                dic.Add("message", exception.Message);
            }
            ret.Data = dic;
            return ret;
        }
    }
}