
using InfoEarthFrame.Common;
using InfoEarthFrame.WebApi.Next.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace InfoEarthFrame.WebApi.Next.Config
{

    public class ApiContext
    {
        private static string ApiResultConfigKey
        {
            get
            {
                var lang = HttpContext.Current.Request.GetOwinContext().Request.Headers[ConfigContext.Current.DefaultConfig["CurrentLangKey"]];
                if (string.IsNullOrEmpty(lang))
                {
                    lang = "zh-cn";
                }
                return string.Format("configs/lang.{0}.xml", lang);
            }
        }
        private static readonly System.Web.Caching.Cache Cache = System.Web.HttpContext.Current.Cache;
        private static readonly object FileLock = new object();
        public static ApiProfile<ApiResult> ApiResults
        {
            get
            {
                try
                {
                    var obj = Cache[ApiResultConfigKey];
                    if (obj == null)
                    {
                        lock (FileLock)
                        {
                            obj = Cache[ApiResultConfigKey];
                            if (obj == null)
                            {
                                var xmlPath = System.Web.HttpContext.Current.Server.MapPath("~/" + ApiResultConfigKey);
                                CacheDependency dp = new CacheDependency(xmlPath);//建立缓存依赖项dp
                                using (var fs = new FileStream(xmlPath, FileMode.Open))
                                {
                                    using (var sr = new StreamReader(fs))
                                    {
                                        var apiProfile = XmlConvert.XmlDeserialize<ApiProfile<ApiResult>>(sr.ReadToEnd(), System.Text.Encoding.Default);
                                        Cache.Insert(ApiResultConfigKey, apiProfile, dp);
                                        return apiProfile;
                                    }
                                }
                            }
                        }
                    }
                    return (ApiProfile<ApiResult>)obj;
                }
                catch (Exception ex)
                {
                    throw new Exception("获取WebApi返回结果配置发生错误，请检查【" + ApiResultConfigKey + "】文件", ex);
                }
            }
        }

        public static string GetText(string moduleName, object key)
        {
            return GetApiResult(moduleName, key).Message;
        }
        public static ApiResult GetApiResult(string moduleName, object key, object data = null)
        {
            try
            {
                var module = ApiResults.Modules.FirstOrDefault(p => p.Name == moduleName);
                module.Items = module.Items.Union(ApiResults.Modules.FirstOrDefault(p => p.Name == "Base").Items).ToList();
                var strKey = Convert.ToString(key);
                bool flag;
                var convertBool = bool.TryParse(strKey, out flag);
                if (convertBool)
                {
                    strKey = flag ? "0" : "-200";
                }
                int code;
                ApiResult result = null;
                if (int.TryParse(strKey, out code))
                {
                    result = module.Items.FirstOrDefault(p => p.Code == code);
                    if (result == null)
                    {
                        throw new Exception(string.Format("文件【{0}】【Name={1}】的Module未能找到【Code={2}】的Result", ApiResultConfigKey, moduleName, code));
                    }
                    result.Data = data;
                    return result;
                }

                result = module.Items.FirstOrDefault(p => p.Message == strKey);
                if (result == null)
                {
                    throw new Exception(string.Format("文件【{0}】【Name={1}】的Module未能找到【Message={2}】的Result", ApiResultConfigKey, moduleName, strKey));
                }
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        
        }
    }
}
