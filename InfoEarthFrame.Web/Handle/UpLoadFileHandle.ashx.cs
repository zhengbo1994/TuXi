using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace InfoEarthFrame.Web.Handle
{
    /// <summary>
    /// UpLoadFileHandle 的摘要说明
    /// 生成上传文件路径
    /// </summary>
    public class UpLoadFileHandle : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string rtnjson = string.Empty;
            Dictionary<string, string> dic = null;
            var ser = new JavaScriptSerializer();
            try
            {
                context.Response.ContentType = "text/plain";
                context.Response.Charset = "utf-8";
                HttpFileCollection fileCollects=context.Request.Files;
                List<HttpPostedFile> files = new List<HttpPostedFile>();
                List<string> filePaths=new List<string>();
                if (fileCollects.Count > 0)
                {
                    DateTime theDate = DateTime.Now;
                    string dt = theDate.Year.ToString() + theDate.Month.ToString() + theDate.Day.ToString() + theDate.Hour.ToString() + theDate.Minute.ToString() + theDate.Second.ToString();
                    string filePath = HttpContext.Current.Server.MapPath("\\file" + "\\" + dt) + "\\";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    for (int i = 0; i < fileCollects.Count; i++) {
                        HttpPostedFile file = fileCollects.Get(i);
                        
                        string fileExtension = Path.GetExtension(file.FileName);
                        var path = filePath + file.FileName.Replace(fileExtension, "") + fileExtension;// + DateTime.Now.ToString("yyyyMMddhhmmss")
                        file.SaveAs(path);
                        filePaths.Add(path);
                    }
    
                }
                else {
                    dic = new Dictionary<string, string>();
                    dic.Add("Success", "false");
                    dic.Add("Message", "请选择需要上传的文件");
                    rtnjson = ser.Serialize(dic);
                    context.Response.Write(rtnjson);
                }

                object resultObject = new { Success = true, Message = "文件保存成功", Filepath = filePaths };
    
                rtnjson = ser.Serialize(resultObject);
                context.Response.Write(rtnjson);
            }
            catch (Exception exception)
            {
                dic = new Dictionary<string, string>();
                dic.Add("Success", "false");
                dic.Add("Message", exception.Message);
                rtnjson = ser.Serialize(dic);
                context.Response.Write(rtnjson);
            }
           
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