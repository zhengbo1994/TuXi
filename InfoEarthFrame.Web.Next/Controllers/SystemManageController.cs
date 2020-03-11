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
    public class SystemManageController : BaseController
    {
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult UserGroupPermession()
        {
            return View();
        }

        public ActionResult AddOrEditUser()
        {
            return View();
        }

        //系统日志
        public ActionResult logger()
        {
            return View();
        }

        public ActionResult Geologger()
        {
            return View();
        }


        public ActionResult PublishService()
        {
            return View();
        }

        public ActionResult AddOrEditService()
        {
            return View();
        }

        public ActionResult UserBaseInfo()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult UserGroupList()
        {
            return View();
        }

        public ActionResult StyleManager()
        {
            return View();
        }

        public ActionResult AddOrEditStyleManager()
        {
            return View();
        }

        public ActionResult SelectServiceType()
        {
            return View();
        }

        public ActionResult Menulist()
        {
            return View();
        }

        public ActionResult SelectMenuType()
        {
            return View();
        }

        public ActionResult AddOrEditMenu()
        {
            return View();
        }

        public ActionResult SelectIcon()
        {
            return View();
        }
    }
}
