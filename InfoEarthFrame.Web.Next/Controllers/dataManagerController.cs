using InfoEarthFrame.Application;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Controllers
{
    public class DataManagerController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddOrEditType()
        {
            return View();
        }

        public ActionResult SelectPackageSource()
        {
            return View();
        }

        public ActionResult AddOrEditPackage()
        {
            return View();
        }

        public ActionResult AddOrEditRarPackage()
        {
            return View();
        }


        public new ActionResult File(string mainId, string folderName)
        {
            if (folderName.EndsWith("图层"))
            {
                ViewBag.Header = "支持MapGIS或ArcGIS数据格式的文件包上传(*.zip|*.rar)";
                ViewBag.UploadUrl = "/DataManager/UploadLayerFile";
                ViewBag.Ext = "zip|rar";
                ViewBag.UploadFailureCallback = "uploadLayerFailureCallback";
            }
            else if (folderName.EndsWith("制图文件") || folderName.EndsWith("系统库"))
            {
                ViewBag.Header = "支持任意文件上传";
                ViewBag.UploadUrl = "/DataManager/UploadFile";
                ViewBag.Ext = "";
            }
            else if (folderName.EndsWith("文档"))
            {
                ViewBag.Header = "支持Word或Excel文件格式上传(*.doc|*.docx|*.xls|*.xlsx)";
                ViewBag.UploadUrl = "/DataManager/UploadFile";
                ViewBag.Ext = "doc|docx|xls|xlsx";

                return View("~/Views/DataManager/Document.cshtml");
            }
            else if (folderName.EndsWith("说明书"))
            {
                ViewBag.Header = "支持Word文件格式上传(*.doc|*.docx)";
                ViewBag.UploadUrl = "/DataManager/UploadFile";
                ViewBag.Ext = "doc|docx";

                return View("~/Views/DataManager/Document.cshtml");
            }
            else if (folderName.EndsWith("栅格图"))
            {
                ViewBag.Header = "支持图片文件格式上传(*.jpg|*.gif|*.png|*.jpeg|*.bmp)";
                ViewBag.UploadUrl = "/DataManager/UploadFile";
                ViewBag.Ext = "jpg|gif|png|jpeg|bmp";

                ViewBag.Url = GetApiUrl("/DataManager/UploadFile") + "?ftpName=layer&mainId=" + mainId + "&folderName=" + HttpUtility.UrlEncode(folderName);

                return View("~/Views/DataManager/Image.cshtml");
            }
            return View();
        }

        public ActionResult MetaData(string mainId, string folderName)
        {
            return View();
        }

        public ActionResult ErrorDetail(string mainId)
        {
            return View();
        }

        public ActionResult UploadMetaDataFile()
        {
            return View();
        }
    }
}
