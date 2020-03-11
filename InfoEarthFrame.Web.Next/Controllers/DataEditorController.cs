using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next.Controllers
{
    public class DataEditorController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowMap(string mainId="",string layerNames="")
        {
            ViewBag.MainId = mainId;
            ViewBag.LayerNames = HttpUtility.UrlDecode(layerNames);
            return View();
        }

 

        public ActionResult StyleList()
        {
            return View();
        }

        public ActionResult EditAttribute()
        {
            return View();
        }


        public ActionResult AddAttribute()
        {
            return View();
        }

    }
}
