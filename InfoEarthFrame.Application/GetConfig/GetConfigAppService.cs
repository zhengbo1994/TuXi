using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using Abp.Application.Services;

namespace InfoEarthFrame.Application.GetConfig
{
    public class GetConfigAppService : ApplicationService, IGetConfigAppService
    {
        public string GetGeoServiceDictionary()
        {
            string GeoServerIp = ConfigurationManager.AppSettings["GeoServerIp"].ToString();
            string GeoServerPort = ConfigurationManager.AppSettings["GeoServerPort"].ToString();
            string GeoWorkSpace = ConfigurationManager.AppSettings["GeoWorkSpace"].ToString();
            string GeoiTelluro = ConfigurationManager.AppSettings["PublishAddress"].ToString();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("GEOSERVERIP", GeoServerIp);
            dict.Add("GEOSERVERPORT", GeoServerPort);
            dict.Add("GEOWORKSPACE", GeoWorkSpace);
            dict.Add("GEOITELLURO", GeoiTelluro);

            return JsonConvert.SerializeObject(dict);
        }

        /// <summary>
        /// 根据节点名称获取web.config 配置文件 AppSettings 节点相关信息
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns></returns>
        public string GetAppSettingsNode(string nodeName)
        {
            return ConfigurationManager.AppSettings.Get(nodeName);
        }

        /// <summary>
        /// 获取缩略图存放地址相对路径
        /// </summary>
        /// <returns></returns>
        public string GetMapThumbnailRelativelyPath()
        {
            string relativelyPath = ConfigurationManager.AppSettings["ThumbnailPath"].ToString().Replace(AppDomain.CurrentDomain.BaseDirectory, "");

            return relativelyPath + "/map";
        }
    }
}

