using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InfoEarthFrame.Common;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System;
using System.Configuration;
using InfoEarthFrame.Application;

namespace InfoEarthFrame.Web.Controllers
{
    //[Authorize]
    public class MultimediaUploadController : InfoEarthFrameControllerBase
    {   
        [HttpPost]
        public ActionResult index()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            JsonResult ret = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            if (string.IsNullOrWhiteSpace(Request["fkey"]) || string.IsNullOrWhiteSpace(Request["typecode"]))
            {
                dic.Add("success", "false");
                dic.Add("message", "检测到数据fkey或者typecode存在空值，终止上传");
                ret.Data = dic;
                return ret;
            }

            try
            {
                Response.ContentType = "text/plain";
                Response.Charset = "utf-8";
                HttpPostedFileBase file = Request.Files["file"];
                string filePath = Server.MapPath("\\file") + "\\";

                if (file != null)
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
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
                string guid = Guid.NewGuid().ToString();
                string newName = guid + fi.Extension;
                string newFile = ConfigurationManager.AppSettings["UploadFilePath"] + newName;
                System.IO.File.Copy(path, newFile);

                //Office转PDF
                if (fileExtension == ".doc" || fileExtension == ".xls" || fileExtension == ".ppt" || fileExtension == ".docx" || fileExtension == ".xlsx" || fileExtension == ".pptx")
                {
                    //变换扩展名
                    string pdfName = guid + ".pdf";
                    string pdfPath = ConfigurationManager.AppSettings["UploadFilePath"] + pdfName;

                    OfficeToPdf topdf = new OfficeToPdf(newFile, pdfPath, fileExtension);                   
                }



                //写数据库
                MultimediaAttachmentInput input = new MultimediaAttachmentInput
                {
                    FKey = Request["fkey"],
                    TypeCode = Request["typecode"],
                    PhysicalName = fileName,
                    LogicName = newName,
                    PhysicalPath = newFile,
                    HttpPath = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + newFile.Replace(Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath), "").Replace(@"\", @"/"),
                    FileSize = file.ContentLength,
                    Extension = fileExtension
                };
                var attachmentAppService = IocResolver.Resolve<AttachmentAppService>();
                string res = attachmentAppService.InsertMultimedia(input);

                if (string.IsNullOrWhiteSpace(res))
                {
                    dic.Add("success", "false");
                    dic.Add("message", "文件保存到数据库失败");
                    ret.Data = dic;
                    return ret;
                }
                
                dic.Add("success", "true");
                dic.Add("message", "文件保存成功");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("id", res);
                data.Add("fKey", input.FKey);
                data.Add("typeCode", input.TypeCode);
                data.Add("physicalName", input.PhysicalName);
                data.Add("logicName", input.LogicName);
                data.Add("physicalPath", input.PhysicalPath);
                data.Add("httpPath", input.HttpPath);
                data.Add("fileSize", input.FileSize.ToString());
                data.Add("extension", input.Extension);

                dic.Add("data", data);
            }
            catch (Exception exception)
            {
                dic = new Dictionary<string, object>();
                dic.Add("success", "false");
                dic.Add("message", exception.Message);
            }            
            ret.Data = dic;
            return ret;
        }
    }
}