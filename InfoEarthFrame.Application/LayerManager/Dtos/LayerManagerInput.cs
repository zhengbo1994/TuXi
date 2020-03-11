using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application
{
    public class LayerManagerInput : IInputDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 标题文本
        /// </summary>
        public string LABEL { get; set; }
        /// <summary>
        /// 放大总级数
        /// </summary>
        public int? ZOOMLEVEL { get; set; }
        /// <summary>
        /// 图层url
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 数据服务Key值
        /// </summary>
        public string DATASERVERKEY { get; set; }
        /// <summary>
        /// 切片大小
        /// </summary>
        public int? TILESIZE { get; set; }
        /// <summary>
        /// 零级大小
        /// </summary>
        public string ZEROLEVELSIZE { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public string PICTYPE { get; set; }
    }
}
