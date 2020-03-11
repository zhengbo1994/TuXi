using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace InfoEarthFrame.Web.Handle
{
    /// <summary>
    /// DownloadFile 的摘要说明
    /// </summary>
    public class DownloadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string filePath = context.Request.QueryString["filename"];
            string[] tempFile = filePath.Split('\\');
            string fileName = tempFile[tempFile.Length - 1];
            fileName = fileName.Replace("[", "_");
            fileName = fileName.Replace("]", "_");
            //string filePath = System.Web.HttpContext.Current.Server.MapPath(fileName);

            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            context.Response.BinaryWrite(bytes);
            context.Response.Flush();
            context.Response.End();
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