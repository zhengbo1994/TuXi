using InfoEarthFrame.ServerInterfaceApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;

namespace InfoEarthFrame.Web.Next.Service.GIS
{
    /// <summary>
    /// gis 的摘要说明
    /// </summary>
    public class gis : IHttpHandler
    {
        private readonly IDictionary<Format, string> _FormatLookup = new Dictionary<Format, string>()
        {
            {Format.png, "image/png"},
            {Format.jpeg, "image/jpeg"}
        };
        public void ProcessRequest(HttpContext context)
        {
            IServerInterfaceAppService server =MvcApplication.DIContainer.Resolve<IServerInterfaceAppService>();
            string mapName = context.Request.QueryString["T"];
            string tileMatrix = context.Request.QueryString["L"];
            string tileCol = context.Request.QueryString["X"];
            string tileRow = context.Request.QueryString["Y"];

            int a1 = 180;
            double b1 = 36;
            int tileLevel = -1;
            if (int.TryParse(tileMatrix, out tileLevel))
            {
                double d1 = 0;
                if (tileLevel == 0)
                {
                    d1 = b1;
                }
                else
                {
                    d1 = b1 / Math.Pow(2, tileLevel);
                }
                int totalRow = Convert.ToInt32(a1 / d1) - 1;

                tileRow = (totalRow - Convert.ToInt32(tileRow)).ToString();
            }

            if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(tileMatrix) || string.IsNullOrEmpty(tileCol) || string.IsNullOrEmpty(tileRow))
            {
                End(context);
            }
            else
            {
                byte[] buffer = server.GetProcessFile(mapName, tileMatrix, tileCol, tileRow);

                if (buffer != null)
                {
                    context.Response.Clear();
                    context.Response.ContentType = _FormatLookup[Format.png];
                    context.Response.BinaryWrite(buffer);
                    context.Response.Flush();
                    context.Response.End();
                }
                else
                {
                    End(context);
                }
            }
        }

        void End(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = 0x194;
            context.Response.Status = "404 Not Found";
            context.Response.ContentType = "text/html";
            context.Response.Write("404 Not Found");
            context.Response.End();
        }

        public enum Format
        {
            png,
            jpeg
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}