using System;
using Abp.Web;
using Castle.Facilities.Logging;
using System.IO;
using System.Text;
using Castle.Windsor;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Web
{
    public class MvcApplication : AbpWebApplication<InfoEarthFrameWebModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            var aa = ColorRampPlusStatic.ColorRampPlus;
            IocContainerHelper.IocContainer = AbpBootstrapper.IocManager.IocContainer;
            AbpBootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));
            base.Application_Start(sender, e);
        }

        protected override void Application_Error(object sender, EventArgs e)
        {
            StreamWriter sw = null;

            try
            {
                string FilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"] + DateTime.Now.ToString("yyyyMMdd") + ".TXT";

                bool bAppend = File.Exists(FilePath);


                sw = new StreamWriter(FilePath, bAppend);
                Exception ex = Server.GetLastError().GetBaseException();
                StringBuilder strErr = new StringBuilder();
                strErr.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                string ip = "";
                if(Request.ServerVariables.Get("HTTP_X_FORWARDED") != null)
                {
                    ip = Request.ServerVariables.Get("HTTP_X_FORWARDED").ToString().Trim();
                }
                else
                {
                    ip = Request.ServerVariables.Get("Remote_Addr").Trim();
                }

                strErr.AppendLine("IP：" + ip);
                strErr.AppendLine("浏览器：" + Request.Browser.Browser.ToString());
                strErr.AppendLine("浏览器版本：" + Request.Browser.MajorVersion.ToString());
                strErr.AppendLine("操作系统：" + Request.Browser.Platform);
                strErr.AppendLine("页面：" + Request.Url.ToString());
                strErr.AppendLine("错误信息：" + ex.Message);
                strErr.AppendLine("错误源：" + ex.Source);
                strErr.AppendLine("异常方法：" + ex.TargetSite);
                strErr.AppendLine("堆栈信息：" );
                strErr.AppendLine(ex.StackTrace);
                strErr.AppendLine("");
                strErr.AppendLine("");

                sw.WriteLine(strErr.ToString());
            }
            catch
            { }
            finally
            {
                Server.ClearError();
                if(sw != null)
                {
                    sw.Close();
                }
            }

            base.Application_Error(sender, e);
        }
    }

    public static class IocContainerHelper
    {
        public static IWindsorContainer IocContainer;
    }
}
