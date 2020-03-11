using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class GroupWithRightEntity
    {        
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
        
        /// <summary>
        /// 图层ID
        /// </summary>
        public string LayerId { get; set; }
        /// <summary>
        /// 是否下载（1是，0否）
        /// </summary>
        public int IsDownload { get; set; }
        /// <summary>
        /// 是否浏览（1是，0否）
        /// </summary>
        public int IsBrowse { get; set; }
    }
}
