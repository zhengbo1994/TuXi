using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Common.ShpUtility;
using InfoEarthFrame.Core;
using InfoEarthFrame.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Controllers
{
    public class ToolsController : BaseController
    {
        //

        public ActionResult Index()
        {
            return View();
        }

        //格式转换
        public ActionResult FormatConversion()
        {
            return View();
        }

        //坐标转换
        public ActionResult CoordinateConversion()
        {
            return View();
        }


        //投影转换
        public ActionResult ShadowConversion()
        {
            return View();
        }


        public ActionResult ConvertMapGISFile(string uploadUrl, string ext,string mainId,string okCallback)
        {
            return base.UploadFile(mainId, "", uploadUrl, ext, okCallback);
        }

        public ActionResult ConvertFormat(string uploadUrl, string ext, string mainId, string okCallback)
        {
            return base.UploadFile(mainId, "", uploadUrl, ext, okCallback);
        }


        public ActionResult ConvertCoordinate(string uploadUrl, string ext, string mainId, string okCallback)
        {
            return base.UploadFile(mainId, "", uploadUrl, ext, okCallback);
        }

        public ActionResult ConvertShadow(string uploadUrl, string ext, string mainId, string okCallback)
        {
            return base.UploadFile(mainId, "", uploadUrl, ext, okCallback);
        }

        public ActionResult ConvertOption()
        {
            return View();
        }
    }
}
