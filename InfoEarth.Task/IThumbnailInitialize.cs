using System.ComponentModel;
using Hangfire.Server;
using Hangfire;


namespace InfoEarth.Task
{
    public interface IThumbnailInitialize
    {
        /// <summary>
        /// 图层与地图初始化下载(空间数据管理平台)
        /// </summary>
        /// <param name="context"></param>
        [AutomaticRetry(Attempts = 2)]
        [DisplayName("图层与地图初始化下载(空间数据管理平台)")]
        [Queue("jobs")]
        void ExcuteJob(PerformContext context = null);
    }
}
