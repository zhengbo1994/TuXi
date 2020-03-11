using System.ComponentModel;
using Hangfire.Server;
using Hangfire;

namespace InfoEarth.Task
{
    public interface IShpFileRead
    {
        /// <summary>
        /// 矢量文件解析服务(空间数据管理平台)
        /// </summary>
        /// <param name="context"></param>
        [AutomaticRetry(Attempts = 2)]
        [DisplayName("矢量文件解析服务")]
        [Queue("jobs")]
        void ExcuteJob(PerformContext context = null);
    }
}
