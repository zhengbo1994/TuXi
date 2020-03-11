using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using InfoEarthFrame.Core.Repositories;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    public class AreaAppService : ApplicationService, IAreaAppService
    {
        #region 变量
        private readonly ISystemUserRepository _ISystemUserRepository;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public AreaAppService(ISystemUserRepository iSystemUserRepository)
        {
            _ISystemUserRepository = iSystemUserRepository;
        }
        #endregion

        #region 根据用户id返回对应行政区划信息
        /// <summary>
        /// 根据用户id返回对应行政区划信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public List<AreaOutput> GetAreaInfoByUserCode(string userCode)
        {
            List<AreaOutput> list = new List<AreaOutput>();
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                if (userCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMNAME))
                {
                    list = LoadAreaXmlByAreaCode(ConstHelper.CONST_SYSTEMCODE);
                }
                else
                {
                    var data = _ISystemUserRepository.FirstOrDefault(s => s.UserCode.Equals(userCode));
                    if (data != null && !string.IsNullOrWhiteSpace(data.Department))
                    {
                        list = LoadAreaXmlByAreaCode(data.Department);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        #endregion

        #region 根据用户id返回对应行政区划信息
        /// <summary>
        /// 根据用户id返回对应行政区划信息
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public List<AreaOutput> GetAreaInfoByAreaCode(string areaCode)
        {
            List<AreaOutput> list = new List<AreaOutput>();
            try
            {
                if (string.IsNullOrWhiteSpace(areaCode))
                {
                    return null;
                }

                if (areaCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMCODE))
                {
                    list = LoadAreaXmlByAreaCode(ConstHelper.CONST_SYSTEMCODE);
                }
                else
                {
                    list = LoadAreaXmlByAreaCode(areaCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        #endregion

        #region 根据用户id返回对应行政区划信息
        /// <summary>
        /// 根据用户id返回对应行政区划信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public List<Area> GetAreaListInfoByUserCode(string userCode)
        {
            var vlist = new List<Area>();
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                var list = GetAreaInfoByUserCode(userCode);
                if (list != null && list.Count > 0)
                {
                    if (userCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMNAME))
                    {
                        RecursiveAreaData(list, vlist);
                    }
                    else
                    {
                        var data = _ISystemUserRepository.FirstOrDefault(s => s.UserCode.Equals(userCode));
                        if (data != null && !string.IsNullOrWhiteSpace(data.Department))
                        {
                            if (data.Department.ToUpper().Equals(ConstHelper.CONST_SYSTEMCODE))
                            {
                                RecursiveAreaData(list, vlist);
                            }
                            else
                            {
                                RecursiveAreaData(list, vlist, data.Department);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return vlist;
        }
        #endregion

        #region 根据用户id返回对应行政区划信息
        /// <summary>
        /// 根据用户id返回对应行政区划信息
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public List<Area> GetAreaListInfoByAreaCode(string areaCode)
        {
            var vlist = new List<Area>();
            try
            {
                if (string.IsNullOrWhiteSpace(areaCode))
                {
                    return null;
                }

                var list = GetAreaInfoByAreaCode(areaCode);
                if (list != null && list.Count > 0)
                {
                    if (areaCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMCODE))
                    {
                        RecursiveAreaData(list, vlist);
                    }
                    else
                    {
                        RecursiveAreaData(list, vlist, areaCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return vlist;
        }
        #endregion

        #region 根据行政区划CODE 获取对应节点数据
        /// <summary>
        /// 根据行政区划CODE 获取对应节点数据
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        private List<AreaOutput> LoadAreaXmlByAreaCode(string areaCode)
        {
            try
            {
                XElement xele = XElement.Load(ConfigHelper.AreaFilePath);
                List<AreaOutput> list = new List<AreaOutput>();
                var nodelist = xele.Descendants("node");
                if (areaCode == ConstHelper.CONST_SYSTEMCODE)
                {
                    #region 超级用户
                    list.AddRange(RecursiveData(nodelist, new List<Area>()));
                    #endregion
                }
                else
                {
                    #region 非超级用户
                    var data = nodelist.Where(s => s.ToString().Contains(areaCode));
                    list.AddRange(RecursiveData(data, new List<Area>(), areaCode, false));
                    #endregion
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 递归获取数据
        /// <summary>
        /// 递归获取数据
        /// </summary>
        /// <param name="enumerable">元素数据</param>
        /// <param name="allList">已添加的列表</param>
        /// <param name="areaCode">区域编号</param>
        /// <param name="status">上级是否匹配</param>
        /// <returns></returns>
        private List<AreaOutput> RecursiveData(IEnumerable<XElement> enumerable, List<Area> allList, string areaCode = "", bool status = false)
        {
            var childlist = new List<AreaOutput>();
            foreach (XElement item in enumerable)
            {
                if (item.HasElements)
                {
                    var obj = GetAreaOutput(item);
                    var childData = RecursiveData(item.Descendants("node"), allList, areaCode, status ? status : string.IsNullOrWhiteSpace(areaCode) ? true : areaCode.Equals(obj.Code));
                    if (childData != null && childData.Count > 0)
                    {
                        obj.Children = childData;
                        allList.Add(new Area() { Code = obj.Code, Label = obj.Label });
                        childlist.Add(obj);
                    }
                }
                else
                {
                    var obj = GetAreaOutput(item);
                    if ((status || string.IsNullOrWhiteSpace(areaCode)) 
                    || (!string.IsNullOrWhiteSpace(areaCode) && areaCode.Equals(obj.Code)))
                    {
                        if (!allList.Any(s => s.Code.Equals(obj.Code)))
                        {
                            allList.Add(new Area() { Code = obj.Code, Label = obj.Label });
                            childlist.Add(obj);
                        }
                    }
                }
            }
            return childlist;
        }
        #endregion

        #region 递归获取数据
        /// <summary>
        /// 递归获取数据
        /// </summary>
        /// <param name="list">元素数据</param>
        /// <param name="allList">已添加的列表</param>
        /// <param name="areaCode">区域编号</param>
        /// <param name="status">上级是否匹配</param>
        /// <returns></returns>
        private void RecursiveAreaData(List<AreaOutput> list, List<Area> childlist, string areaCode = "", bool status = false)
        {
            foreach (AreaOutput item in list)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    RecursiveAreaData(item.Children, childlist, areaCode, status ? status : string.IsNullOrWhiteSpace(areaCode) ? true : areaCode.Equals(item.Code));
                    if ((status || string.IsNullOrWhiteSpace(areaCode))
                    || (!string.IsNullOrWhiteSpace(areaCode) && areaCode.Equals(item.Code)))
                    {
                        childlist.Add(new Area() { Code = item.Code, Label = item.Label });
                    }
                }
                else
                {
                    if ((status || string.IsNullOrWhiteSpace(areaCode))
                    || (!string.IsNullOrWhiteSpace(areaCode) && areaCode.Equals(item.Code)))
                    {
                        childlist.Add(new Area() { Code = item.Code, Label = item.Label });
                    }
                }
            }
        }
        #endregion

        #region 根据元素节点,上级code 赋值对象
        /// <summary>
        /// 根据元素节点,上级code 赋值对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private AreaOutput GetAreaOutput(XElement data)
        {
            try
            {
                AreaOutput obj = new AreaOutput();
                obj.Label = data.Attribute("Name").Value;
                obj.Code = data.Attribute("Code").Value;
                return obj;
            }
            catch { return null; }
        }
        #endregion

        #region 根据元素节点,上级code 赋值对象
        /// <summary>
        /// 根据元素节点,上级code 赋值对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Area GetArea(XElement data)
        {
            try
            {
                Area obj = new Area();
                obj.Label = data.Attribute("Name").Value;
                obj.Code = data.Attribute("Code").Value;
                return obj;
            }
            catch { return null; }
        }
        #endregion
    }
}
