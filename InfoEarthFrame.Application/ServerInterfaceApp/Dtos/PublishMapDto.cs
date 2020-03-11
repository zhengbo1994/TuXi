using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    /// <summary>
    /// 地图信息
    /// </summary>
   public class PublishMapDto
    {
        /// <summary>
        /// 图层文件
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName { get; set; }
        /// <summary>
        /// 图层分类
        /// </summary>
        public string LayerType { get; set; }
        /// <summary>
        /// 地图名称
        /// </summary>
        public string MapName { get; set; }
        /// <summary>
        /// 地图分类
        /// </summary>
        public string MapType { get; set; }
    }
}
