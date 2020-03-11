using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using iTelluro.SYS.Entity;

namespace InfoEarthFrame.Application
{
    public interface IAreaAppService : IApplicationService
    {
        /// <summary>
        /// 根据用户id返回对应行政区划层级信息
        /// </summary>
        /// <param name="userCode">用户id</param>
        /// <returns></returns>
        List<AreaOutput> GetAreaInfoByUserCode(string userCode);

        /// <summary>
        /// 根据行政区划返回对应行政区划层级信息
        /// </summary>
        /// <param name="userCode">行政区划id</param>
        /// <returns></returns>
        List<AreaOutput> GetAreaInfoByAreaCode(string areaCode);

        /// <summary>
        /// 根据用户id返回对应行政区划列表信息
        /// </summary>
        /// <param name="userCode">用户id</param>
        /// <returns></returns>
        List<Area> GetAreaListInfoByUserCode(string userCode);

        /// <summary>
        /// 根据行政区划返回对应行政区划层级信息
        /// </summary>
        /// <param name="areaCode">行政区划id</param>
        /// <returns></returns>
        List<Area> GetAreaListInfoByAreaCode(string areaCode);

    }
}
