using System;
using System.Collections.Generic;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    public class KneeCoordinateInput 
    {
        /// <summary>
        /// 坐标系标准
        /// </summary>
        public string CoordSys  { get; set; }  
        /// <summary>
        /// 是否为投影坐标
        /// </summary>
        public bool IsProjectiveGeo { get; set; }
        /// <summary>
        /// 坐标列表
        /// </summary>
        public List<KneeCoordinateModal> InitialData { get; set; }
    }
}
