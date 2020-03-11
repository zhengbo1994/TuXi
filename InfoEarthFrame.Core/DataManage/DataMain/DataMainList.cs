using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Core
{
    public class DataMainList
    {
        /// <summary>
        /// 图系主信息ID
        /// </summary>
        public string DataMainID { get; set; }

        /// <summary>
        /// 元数据ID
        /// </summary>
        public string MetaDataID { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 图片ID
        /// </summary>
        public string ImageID { get; set; }

        /// <summary>
        /// 图系名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 专题类型
        /// </summary>
        public string TPCat { get; set; }

        /// <summary>
        /// 绘图单位
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 摘要信息
        /// </summary>
        public string IdAbs { get; set; }
    }
}
