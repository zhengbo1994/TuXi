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
    public class UploadImageController : InfoEarthFrameControllerBase
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
                string folder = Request.Form["folder"];
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style", folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.Combine(path, file.FileName);

                file.SaveAs(fileName);

                dic.Add("success", "true");
                dic.Add("message", "上传成功!");
            }
            catch (Exception exception)
            {
                dic.Add("success", "false");
                dic.Add("message", exception.Message);
            }
            ret.Data = dic;
            return ret;
        }
    }
}