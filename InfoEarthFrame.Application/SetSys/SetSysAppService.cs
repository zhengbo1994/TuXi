using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.AutoMapper;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Configuration;

namespace InfoEarthFrame.Application
{
    //[AbpAuthorize]
    public class SetSysAppService : ApplicationService, ISetSysAppService
    {
        private string _path = System.AppDomain.CurrentDomain.BaseDirectory + @"setSys.json";
        public SetSysDto GetList()
        {
            if (!File.Exists(_path))
            {
                return null;
            }
            try
            {
                string json = "";
                FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    json += str;
                }
                sr.Close();
                fs.Close();
                SetSysDto ret = new SetSysDto
                {
                    Json = json
                };
                return ret;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public bool Add(SetSysDto input)
        {
            try
            {
                if (!File.Exists(_path))
                {
                    FileStream fs = new FileStream(_path, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(input.Json);
                    sw.Close();
                    fs.Close();
                }
                StreamWriter sr = new StreamWriter(_path);
                sr.WriteLine(input.Json);
                sr.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 更新配置节点
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public bool UpdateAppSetting(string key, string value)
        //{
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();

        //        //获取配置文件的全路径
        //        string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Web.config";

        //        doc.Load(strFileName);

        //        XmlNodeList nodes = doc.GetElementsByTagName("add");

        //        for (int i = 0;i<nodes.Count;i++ )
        //        {
        //            XmlAttribute _key = nodes[i].Attributes["key"];

        //            if(_key != null)
        //            {
        //                if(_key.Value == key)
        //                {
        //                    _key = nodes[i].Attributes["value"];
        //                    _key.Value = value;

        //                    break;
        //                }
        //            }
        //        }

        //        doc.Save(strFileName);
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 获取指定属性值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public string GetAppSettingAttribute(string key)
        //{
        //    string strValue = "";
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();

        //        //获取配置文件的全路径
        //        string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Web.config";

        //        doc.Load(strFileName);

        //        XmlNodeList nodes = doc.GetElementsByTagName("add");

        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            XmlAttribute _key = nodes[i].Attributes["key"];

        //            if (_key != null)
        //            {
        //                if (_key.Value == key)
        //                {
        //                    _key = nodes[i].Attributes["value"];

        //                    strValue = _key.Value;

        //                    break;
        //                }
        //            }
        //        }

        //        return strValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        return strValue;
        //    }
        //}


        /// <summary>
        /// 更新配置节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateAppSetting(string key, string value)
        {
            try
            {
                Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                config.AppSettings.Settings.Remove(key);
                config.AppSettings.Settings.Add(key, value);
                config.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取指定属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAppSettingAttribute(string key)
        {
            string strValue = "";
            try
            {
                strValue = ConfigurationManager.AppSettings.Get(key);

                return strValue;
            }
            catch (Exception ex)
            {
                return strValue;
            }
        }

    }
}
