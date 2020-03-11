
using InfoEarthFrame.Common;
using InfoEarthFrame.WebApi.Next.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Xml.Serialization;

namespace InfoEarthFrame.WebApi.Next.Config
{

    public class ApiAuthorizeManager
    {
        private const string ApiAuthorizeConfigKey = "configs/ApiAuthorize.xml";
        private static readonly System.Web.Caching.Cache Cache = System.Web.HttpContext.Current.Cache;
        private static readonly object FileLock = new object();
        public static ApiProfile<ApiAuthorize> ApiAuthorizes
        {
            get
            {
                try
                {
                    
                    var obj = Cache[ApiAuthorizeConfigKey];
                    if (obj == null)
                    {
                        lock (FileLock)
                        {
                             obj = Cache[ApiAuthorizeConfigKey];
                             if (obj == null)
                             {
                                 var xmlPath = System.Web.HttpContext.Current.Server.MapPath("~/" + ApiAuthorizeConfigKey);
                                 CacheDependency dp = new CacheDependency(xmlPath);//建立缓存依赖项dp
                                 using (var fs = new FileStream(xmlPath, FileMode.Open))
                                 {
                                     using (var sr = new StreamReader(fs))
                                     {
                                         var apiProfile = XmlConvert.XmlDeserialize<ApiProfile<ApiAuthorize>>(sr.ReadToEnd(), System.Text.Encoding.Default);
                                         Cache.Insert(ApiAuthorizeConfigKey, apiProfile, dp);
                                         return apiProfile;
                                     }
                                 }
                             }
                        }
                    }
                    return (ApiProfile<ApiAuthorize>)obj;
                }
                catch (Exception ex)
                {
                    throw new Exception("获取WebApi授权配置发生错误，请检查【" + ApiAuthorizeConfigKey + "】文件", ex);
                }
            }
        }

        public static bool IsPageValid(string requestPath, Func<bool> ValidTokenFunc)
        {
            var hasAuthPage = false;
            if (ApiAuthorizes != null
                && ApiAuthorizes.Modules.Any())
            {
                foreach (var module in ApiAuthorizes.Modules)
                {
                    if (module.Items != null
                        && module.Items.Any())
                    {
                        foreach (var auth in module.Items)
                        {
                            if (requestPath.ToLower().StartsWith(auth.Page.ToLower()))
                            {
                                hasAuthPage = true;
                                //不需要令牌
                                if (!auth.NeedToken)
                                {
                                    return true;
                                }

                                if (ValidTokenFunc != null)
                                {
                                    return ValidTokenFunc();
                                }

                                return false;
                            }
                        }
                    }
                }
            }
            if (!hasAuthPage)
            {
                throw new Exception("未能找到WebApi【" + requestPath + "】的授权配置，请检查【" + ApiAuthorizeConfigKey + "】文件");
            }
            return false;
        }

    }
}