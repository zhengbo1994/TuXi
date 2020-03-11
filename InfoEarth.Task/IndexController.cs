using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace InfoEarth.Task
{
    public class IndexController : ApiController
    {
        [HttpGet]
        public string GetInfo()
        {
            BackgroundJob.Enqueue<ShpFileRead>(x => x.ExcuteJob(null));
            return "ok";
        }


        /// <summary>
        /// 矢量文件解析服务(空间数据管理平台)
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public string VecFileExcuteJob()
        //{
        //    return "VecFileExcuteJob";
        //    //try
        //    //{
        //    //    BackgroundJob.Enqueue<ShpFileRead>(x => x.ExcuteJob(null));
        //    //    //string cron = string.Empty;
        //    //    //RecurringJob.AddOrUpdate<IShpFileRead>(string.Format("矢量文件解析服务"), x => x.ExcuteJob(null), Cron.Daily, TimeZoneInfo.Local);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    //return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    //}
        //    //return Request.CreateResponse(HttpStatusCode.OK);
        //}



    }
}
