using InfoEarthFrame.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(TestController));
        [AllowAnonymous]
        public IHttpActionResult GetLogTest()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 授权测试
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetGreeting()
        {
            var claims = Request.GetOwinContext().Authentication.User.Claims.Select(p => new KeyValuePair<string, object>(p.Type, p.Value));
            var auth= Request.GetOwinContext().Authentication.User.Identity.IsAuthenticated;
            return Ok(new
            {
                claims = claims,
                auth = auth,
                language = CurrentLang
            });
        }

        public IHttpActionResult GetCreateFtpDirResult()
        {
            var ftp = ConfigContext.Current.FtpConfig["package"];
            using (var util = new FtpHelper(ftp))
            {
                    util.MkDir(@"Package\95eb24e4-eb44-44c1-9f0a-91f1902b06de\6要素类文件\地理图层");
            }

            return Ok();
        }

        public IHttpActionResult GetMap2ShpPathResult(string fileName)
        {
            var s = Mapgis2Arcgis(fileName);
            return Ok(s);
        }

        public string[] Mapgis2Arcgis(string filename)
        {
            List<string> filenames = new List<string>();

            //Process p = null;
            DateTime dtStart = DateTime.Now;
            object fileName = filename;
            if (fileName == null || string.IsNullOrEmpty(fileName.ToString()))
            {
                return null;
            }
            Environment.CurrentDirectory = ConfigurationManager.AppSettings["Map2ShpPath"];
           
            string dir = Path.Combine(Path.GetDirectoryName(filename), "ShapeData"); ;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                _logger.Debug(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe" + " Map2Shp -s:" + fileName + " -t:" + dir);
                //Process.EnterDebugMode(); // 等待完成
                Process.Start(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe", " Map2Shp -s:" + fileName + " -t:" + dir);

                DirectoryInfo folder = new DirectoryInfo(dir);
                foreach (FileInfo file in folder.GetFiles())
                {
                    filenames.Add(file.FullName);
                }
                string[] filenameslst = (string[])filenames.ToArray();
                return filenameslst;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }
    }
}
