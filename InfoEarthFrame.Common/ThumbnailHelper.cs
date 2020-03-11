using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Net;

namespace InfoEarthFrame.Common
{
    public class ThumbnailHelper
    {
        private string _GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"]
                 , _GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"]
                 , _GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"]
                 , _GeoEPSG = ConfigurationManager.AppSettings["EPSG"]
                 , GEOSERVER_USERNAME = ConfigurationManager.AppSettings["GeoServerUser"]
                 , GEOSERVER_PASSWORD = ConfigurationManager.AppSettings["GeoServerPwd"];


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public string CreateThumbnail(string imageName, string type, string bbox, string uploadFileType = "", string uploadFileSrs = "")
        {
            var requestUrl = GetWMSRequestUrl(imageName, bbox, uploadFileType, uploadFileSrs);
            var byteImage = GetImageByte(requestUrl);
            return SaveThumbnail(byteImage, type, imageName);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SaveThumbnail(byte[] thumbnail, string type, string fileName)
        {
            string filePath = "";
            try
            {
                string filderPath = System.Configuration.ConfigurationManager.AppSettings["ThumbnailPath"] + "\\" + type;

                if (!Directory.Exists(filderPath))
                {
                    Directory.CreateDirectory(filderPath);
                }

                filePath = Path.Combine(filderPath + "\\" + fileName + ".png");

                MemoryStream ms = new MemoryStream(thumbnail);
                Image image = Image.FromStream(ms, true);
 
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                return filePath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 组织wms服务URL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public string GetWMSRequestUrl(string name, string bbox, string uploadFileType, string uploadFileSrs)
        {
            string requestUrl = "";
            requestUrl += string.Format("http://{0}:{1}/geoserver/{2}/{3}", _GeoServerIp, _GeoServerPort, _GeoWorkSpace, "wms");
            requestUrl += "?service=WMS";
            requestUrl += "&version=1.1.0";
            requestUrl += "&request=GetMap";
            requestUrl += "&layers=" + (string.IsNullOrEmpty(_GeoWorkSpace) ? name : (_GeoWorkSpace + ":" + name));
            requestUrl += "&bbox=" + bbox;
            requestUrl += "&styles=" + "";
            requestUrl += "&width=" + "196";
            requestUrl += "&height=" + "171";
            requestUrl += "&srs=" + (!string.IsNullOrWhiteSpace(uploadFileType) ? uploadFileType == Convert.ToInt16(DataTypeHelper.影像).ToString() ? !string.IsNullOrWhiteSpace(uploadFileSrs) ? uploadFileSrs : _GeoEPSG : _GeoEPSG : _GeoEPSG);
            requestUrl += "&format=" + ConstHelper.DataFormatHelper[ConstHelper.VECTOR_FORMAT];
            return requestUrl;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public byte[] GetImageByte(string requestUrl)
        {
            HttpWebRequest request = null;
            byte[] result = null;
            int index = 0;
            while (index < 2)
            {
                try
                {
                    System.GC.Collect();
                    request = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
                    request.Method = "GET";
                    request.Credentials = new NetworkCredential(GEOSERVER_USERNAME, GEOSERVER_PASSWORD);
                    request.ContentType = "text/html";
                    request.Accept = "application/octet-stream";
                    request.KeepAlive = false;
                    request.Timeout = 600000;//10分钟
                    MemoryStream ms = new MemoryStream();
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    Stream resStream = response.GetResponseStream();
                    byte[] bytes = new byte[4096];
                    int bytesRead = resStream.Read(bytes, 0, bytes.Length);
                    while (bytesRead > 0)
                    {
                        ms.Write(bytes, 0, bytesRead);
                        bytesRead = resStream.Read(bytes, 0, bytes.Length);
                    }

                    response.Close();
                    response = null;
                    ms.Flush();
                    ms.Seek(0L, SeekOrigin.Begin);
                    result = ms.ToArray();
                    index++;
                    break;
                }
                catch (Exception ex)
                {
                    if (request != null)
                        request.Abort();
                    request = null;

                    index++;
                    if (index < 2)
                        continue;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除缩略图
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteThumbnail(string type, string fileName)
        {
            string filePath = "";
            try
            {
                string filderPath = System.Configuration.ConfigurationManager.AppSettings["ThumbnailPath"] + "\\" + type;

                if (!Directory.Exists(filderPath))
                {
                    Directory.CreateDirectory(filderPath);
                }

                filePath = Path.Combine(filderPath + "\\" + fileName + ".png");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
