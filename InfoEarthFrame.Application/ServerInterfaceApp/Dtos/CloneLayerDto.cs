using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
   public  class CloneLayerDto
    {
        /// <summary>
        /// 图层编号
        /// </summary>
        public string OldLayerId { get; set; }
        /// <summary>
        /// 复制后的图层名称
        /// </summary>
        public string NewLayerName { get; set; }
    }
}
