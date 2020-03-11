using Abp.Application.Services;
using System;

namespace InfoEarthFrame.Application.GetConfig
{
    public interface IGetConfigAppService : IApplicationService
    {
        #region 自动生成
        string GetGeoServiceDictionary();

        /// <summary>
        /// 根据节点名称获取web.config 配置文件 AppSettings 节点相关信息
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        string GetAppSettingsNode(string nodeName);

        /// <summary>
        /// 获取缩略图存放地址相对路径
        /// </summary>
        /// <returns></returns>
        string GetMapThumbnailRelativelyPath();
        #endregion
    }
}

