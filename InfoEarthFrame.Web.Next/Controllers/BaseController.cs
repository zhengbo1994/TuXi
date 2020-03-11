using InfoEarthFrame.Common;
using InfoEarthFrame.Web.Next.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Controllers
{
    [CustomAuthFilterAttribute]
    public class BaseController : System.Web.Mvc.Controller
    {
        public JsonResult2 Json2(object data, JsonRequestBehavior jsonRequestBehavior = JsonRequestBehavior.AllowGet)
        {
            var result = new JsonResult2();
            result.Data = data;
            result.JsonRequestBehavior = jsonRequestBehavior;
            return result;
        }

        [HttpPost]
        public JsonResult ChangeLang()
        {
            var currentLang = UserConfigContext.CurrentLang;
            if (currentLang == "zh-cn")
            {
                UserConfigContext.CurrentLang = "en";
            }
            else
            {
                UserConfigContext.CurrentLang = "zh-cn";
            }
            return Json2(new HttpResponseResult
            {
                Code = 0
            });
        }
     
        public ActionResult SelectTree(string apiPath)
        {
            ViewBag.ApiPath = apiPath;
            return View("~/Views/Shared/_SelectTreeLayout.cshtml");
        }

        public ActionResult UploadFile(string mainId, string folderName, string uploadUrl, string ext, string okCallback, string uploadFailureCallback="")
        {
            ViewBag.Url = GetApiUrl(uploadUrl) + "?mainId=" + mainId + "&folderName=" + HttpUtility.UrlEncode(folderName) + "&okCallback=" + okCallback + "&failureCallback=" + uploadFailureCallback;
            ViewBag.Ext = ext;
            return View("~/Views/Shared/_UploadLayout.cshtml");
        }


        public ActionResult _UserList()
        {
            return View("~/Views/Shared/_UserList.cshtml");
        }

        public JsonResult ChangeLang(string lang)
        {
            UserConfigContext.CurrentLang = lang;
            return Json2(new HttpResponseResult
            {
                Code = 0
            });
        }

        public static string GetApiUrl(string rawUrl)
        {
            return ConfigContext.Current.ApiConfig.BaseUrl+"/api" + rawUrl;
        }

        public JsonResult GetPDFFile(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            var ftp=ConfigContext.Current.FtpConfig["package"];
            var ftpPath=filePath.Replace(ftp.Site, "");
            var pdfPath=ftpPath.Replace(ext, ".pdf");
            pdfPath = TempDirectory + "\\" + pdfPath.Replace("/", "\\");
            pdfPath = pdfPath.Replace("\\\\", "\\");

            //判断PDF文件是否存在
            var file = new FileInfo(pdfPath);
            if (!file.Exists)
            {
                //去FTP下载
                using (var client = new FtpHelper(ftp))
                {
                    var srcPath=pdfPath.Replace(".pdf", ext);
                    client.Get(srcPath, ftpPath);

                    //TODO:注释了
                  PDFConverter.Convert(srcPath, pdfPath);

                }
            }


            return Json2(new HttpResponseResult
            {
                Code = 0,
                Data ="/"+pdfPath.Replace(Server.MapPath("~"), "").Replace("\\","/")
            });
        }



        protected string TempDirectory
        {
            get
            {
                var dir = Path.Combine(Server.MapPath("~"), "TempFile");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
        }
    }

    public class JsonResult2 : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(JsonConvert.SerializeObject(this.Data));
        }
    }
}
